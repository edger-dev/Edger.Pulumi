namespace Edger.Pulumi.Apps;

using Edger.Pulumi;
using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;

public class Prometheus : StatefulApp {
    public new const string Name = "prometheus";
    public const int Port = 9090;

    public const string PvcName = "prometheus-data";
    public const string MountName = "data";
    public const string MountPath = "/data";

    public const string ConfigMapName = "prometheus-config";
    public const string ConfigMountName = "config";
    public const string ConfigMountPath = "/etc/prometheus";

    public static string Image(string version = "v2.38.0") {
        return "prom/prometheus:" + version;
    }

    protected static InputList<VolumeArgs> GetVolumes(
        Namespace ns, string config
    ) {
        var configData = new InputMap<string> {
            { "prometheus.yaml", config }
        };
        var configMap = K8s.ConfigMap(ns, ConfigMapName, configData);
        return new InputList<VolumeArgs> {
            K8s.PvcVolume(MountName, PvcName),
            K8s.ConfigMapVolume(ConfigMountName, ConfigMapName)
        };
    }

    public Prometheus(Namespace ns,
        string image,
        string config,
        string? ingressHost = null,
        string requestSize = "10Gi",
        StorageClass? storageClass = null
    ) : base(ns, Name, "api", Port,
        image, GetPvcs(ns, PvcName, requestSize, storageClass), GetVolumes(ns, config), GetVolumeMounts(MountName, MountPath, ConfigMountName, ConfigMountPath),
        args: new InputList<string> {
            "--config.file=/etc/prometheus/prometheus.yaml",
            "--storage.tsdb.path=/data",
        }
    ) {
        if (ingressHost != null) {
            ApplyHostIngress(ingressHost, Port);
        }
    }
}


