namespace Edger.Pulumi.Apps;

using Edger.Pulumi;
using global::Pulumi;

public class Registry : StatefulApp {
    public new const string Name = "registry";
    public const string Image = "registry:2";
    public const int Port = 5000;

    public const string PvcName = "registry-data";
    public const string MountName = "data";
    public const string MountPath = "/var/lib/grafana";

    public const string LoadBalancerName = "registry-external";

    public readonly Output<string>? LoadBalancerIP;

    public Registry(Namespace ns,
        string? ingressHost = null,
        int? lbPort = null,
        string requestSize = "10Gi",
        StorageClass? storageClass = null
    ) : base(ns, Name, "api", Port, Image,
        GetPvcs(ns, PvcName, requestSize, storageClass),
        GetVolumes(MountName, PvcName),
        GetVolumeMounts(MountName, MountPath)
    ) {
        if (ingressHost != null) {
            ApplyHostIngress(ingressHost, Port);
        }
        if (lbPort != null) {
            var lb = ApplyLoadBalancer(LoadBalancerName, lbPort.Value, Port);
            LoadBalancerIP = lb.LoadBalancerIP();
        }
    }
}