namespace Edger.Pulumi.Apps;

using Edger.Pulumi;
using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;

public class Bytebase : StatefulApp {
    public new const string Name = "bytebase";
    public const int Port = 8080;

    public const string PvcName = "bytebase-data";
    public const string MountPath = "/var/opt/bytebase";

    public const string LoadBalancerName = "bytebase-external";

    public readonly Output<string>? LoadBalancerIP;

    public static string Image(string version = "1.15.0") {
        return "bytebase/bytebase:" + version;
    }

    public static PvcTemplateVolume Volume(
        Namespace ns,
        string requestSize,
        InputMap<string>? labels = null,
        StorageClass? storageClass = null
    ) {
        return new PvcTemplateVolume(ns, PvcName, MountPath, requestSize, labels, storageClass);
    }

    public Bytebase(Namespace ns,
        string image,
        PvcTemplateVolume pvc,
        string pgUrl,
        string? ingressHost = null,
        int? lbPort = null,
        string name = Name
    ) : base(ns, name, "db", Port,
        image,
        new Volume[] {
            pvc
        },
        env: K8s.ContainerEnv(
            ("PG_URL", pgUrl)
        )
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