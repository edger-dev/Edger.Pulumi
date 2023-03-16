namespace Edger.Pulumi.Apps;

using System.Text;
using Edger.Pulumi;
using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;

public class Grafana : StatefulApp {
    public new const string Name = "grafana";
    public const int Port = 3000;

    public const string PvcName = "grafana-data";
    public const string MountPath = "/var/lib/grafana";

    public const string DatasourcesConfigMapName = "grafana-datasources";
    public const string DatasourcesConfigMountName = "datasources";
    public const string DatasourcesConfigMountPath = "/etc/grafana/provisioning/datasources";

    public const string DashboardsConfigMapName = "grafana-dashboards";
    public const string DashboardsConfigMountName = "dashboards";
    public const string DashboardsConfigMountPath = "/etc/grafana/provisioning/dashboards";

    // Put each dashboard into it's own ConfigMap, since k8s can't support big content in one ConfigMap.
    public const string DashboardsConfigMapNamePrefix = "grafana-dashboards-";
    public const string DashboardsConfigMountNamePrefix = "dashboards-";
    public const string DashboardsConfigMountPathPrefix = "/etc/grafana/provisioning/dashboards/";

    public static string Image(string version = "9.1.0-ubuntu") {
        return "grafana/grafana:" + version;
    }

    public static PvcTemplateVolume Volume(
        Namespace ns,
        string requestSize,
        InputMap<string>? labels = null,
        StorageClass? storageClass = null
    ) {
        return new PvcTemplateVolume(ns, PvcName, MountPath, requestSize, labels, storageClass);
    }

    public static string PrometheusDatasource(
        string name = "Prometheus",
        string uid = "prometheus",
        string url = "http://prometheus:9090",
        int intervalSeconds = 15,
        bool isDefault = false
    ) {
        return $@"
apiVersion: 1

datasources:
  - name: {name}
    type: prometheus
    uid: {uid}
    url: {url}
    isDefault: {isDefault}
    jsonData:
      timeInterval: {intervalSeconds}s
";
    }

    public static string DashboardFolder(
        string name = "Default",
        string folder = "Builtin",
        string path = DashboardsConfigMountPath,
        bool foldersFromFilesStructure = false
    ) {
        return $@"
apiVersion: 1

providers:
  - name: '{name}'
    folder: '{folder}'
    type: file
    options:
      path: {path}
      foldersFromFilesStructure: {foldersFromFilesStructure}
";
    }

    private static Volume GetDashboardsVolume(
        Namespace ns
    ) {
        return new ConfigMapVolume(
            ns,
            DashboardsConfigMapName,
            DashboardsConfigMountPath,
            DashboardsConfigMountName,
            ("dashboards.yaml", DashboardFolder())
        );
    }

    private static Volume GetDatasourcesVolume(
        Namespace ns,
        (string, string)[] datasources
    ) {
        return new ConfigMapVolume(
            ns,
            DatasourcesConfigMapName,
            DatasourcesConfigMountPath,
            DatasourcesConfigMountName,
            datasources
        );
    }

    private static Volume GetDashboardVolume(
        Namespace ns,
        string folder,
        string name,
        string content
    ) {
        var filename = name;
        if (!filename.EndsWith(".json")) {
            filename = name + ".json";
        }
        return new ConfigMapVolume(
            ns,
            DashboardsConfigMapNamePrefix + folder,
            DashboardsConfigMountPathPrefix + folder,
            DashboardsConfigMountNamePrefix + folder,
            ( filename, content)
        );
    }

    private static Volume[] GetVolumes(
        Namespace ns,
        PvcTemplateVolume pvc,
        (string, string)[] datasources,
        (string, string, string)[] dashboards
    ) {
        var result = new List<Volume>();
        result.Add(pvc);
        result.Add(GetDatasourcesVolume(ns, datasources));
        result.Add(GetDashboardsVolume(ns));
        foreach (var dashboard in dashboards) {
            result.Add(GetDashboardVolume(ns,
                dashboard.Item1,
                dashboard.Item2,
                dashboard.Item3));
        }
        return result.ToArray();
    }

    public Grafana(Namespace ns,
        string image,
        PvcTemplateVolume pvc,
        (string, string)[] datasources,
        (string, string, string)[] dashboards,
        string? ingressHost = null,
        string name = Name
    ) : base(ns, name, "ui", Port,
        image,
        GetVolumes(ns, pvc, datasources, dashboards)
    ) {
        if (ingressHost != null) {
            ApplyHostIngress(ingressHost, Port);
        }
    }
}