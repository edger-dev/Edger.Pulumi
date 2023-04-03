namespace Edger.Pulumi.Apps;

using Edger.Pulumi;
using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;

public class Bytebase : StatefulApp {
    public const string NAME = "bytebase";
    public const int Port = 80;

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
        string externalUrl,
        string? ingressHost = null,
        int? lbPort = null,
        string name = NAME
    ) : base(ns, name, "db", Port,
        image,
        new Volume[] {
            pvc
        },
        env: K8s.ContainerEnv(
            ("PG_URL", pgUrl)
        ),
        args: new InputList<string> {
            "--data",
            MountPath,
            "--external-url",
            externalUrl,
        }
    ) {
        if (ingressHost != null) {
            ApplyHostIngress(ingressHost, Port);
        }
        if (lbPort != null) {
            var lb = ApplyLoadBalancer(LoadBalancerName, lbPort.Value, Port);
            LoadBalancerIP = lb.LoadBalancerIP();
        }
    }

    public Bytebase(Namespace ns,
        string image,
        PvcTemplateVolume pvc,
        BytebaseDB db,
        string externalUrl,
        string? ingressHost = null,
        int? lbPort = null,
        string name = NAME
    ) : this(ns, image, pvc, db.Url(),
            externalUrl, ingressHost, lbPort, name) {
    }
}