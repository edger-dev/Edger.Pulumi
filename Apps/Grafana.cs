namespace Edger.Pulumi.Apps;

using Edger.Pulumi;

public class Grafana : StatefulApp {
    public new const string Name = "grafana";
    public const int Port = 3000;

    public const string PvcName = "grafana-data";
    public const string MountName = "data";
    public const string MountPath = "/var/lib/grafana";


    public static string Image(string version = "9.1.0-ubuntu") {
        return "grafana/grafana:" + version;
    }

    public Grafana(Namespace ns,
        string image,
        string? ingressHost = null,
        string requestSize = "10Gi",
        StorageClass? storageClass = null
    ) : base(ns, Name, "ui", Port,
        image, GetPvcs(ns, PvcName, requestSize, storageClass), GetVolumes(MountName, PvcName), GetVolumeMounts(MountName, MountPath)
    ) {
        if (ingressHost != null) {
            ApplyHostIngress(ingressHost, Port);
        }
    }
}