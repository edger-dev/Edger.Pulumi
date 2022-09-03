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

    public static string Image(string version = "9.1.0-ubuntu") {
        return "grafana/grafana:" + version;
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

    private static InputList<VolumeArgs> GetVolumes(
        Namespace ns,
        (string, string)[] datasources,
        (string, string)[] dashboards
    ) {
        var datasourcesData = new InputMap<string>();
        foreach (var source in datasources) {
            datasourcesData.Add(source.Item1, source.Item2);
        }
        var datasourcesConfigMap = K8s.ConfigMap(ns, DatasourcesConfigMapName, datasourcesData);

        var dashboardsData = new InputMap<string>();
        foreach (var dashboard in dashboards) {
            dashboardsData.Add(dashboard.Item1, dashboard.Item2);
        }
        var dashboardsConfigMap = K8s.ConfigMap(ns, DashboardsConfigMapName, dashboardsData);

        return new InputList<VolumeArgs> {
            K8s.ConfigMapVolume(DatasourcesConfigMountName, datasourcesConfigMap.Apply(DatasourcesConfigMapName)),
            K8s.ConfigMapVolume(DashboardsConfigMountName, dashboardsConfigMap.Apply(DashboardsConfigMapName))
        };
    }


    public Grafana(Namespace ns,
        string image,
        Pvc pvc,
        (string, string)[] datasources,
        (string, string)[] dashboards,
        string? ingressHost = null
    ) : base(ns, Name, "ui", Port,
        image,
        GetPvcTemplates(pvc),
        GetVolumeMounts(PvcName, MountPath,
            DatasourcesConfigMountName, DatasourcesConfigMountPath,
            DashboardsConfigMountName, DashboardsConfigMountPath
        ),
        GetVolumes(ns, datasources, dashboards)
    ) {
        if (ingressHost != null) {
            ApplyHostIngress(ingressHost, Port);
        }
    }
}