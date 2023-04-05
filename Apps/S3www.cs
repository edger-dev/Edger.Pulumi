namespace Edger.Pulumi.Apps;

using Edger.Pulumi;
using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;

public class S3www : StatelessApp {
    public const string NAME = "s3www";
    public const int Port = 8080;

    public const string LoadBalancerName = "s3www-external";

    public readonly Output<string>? LoadBalancerIP;

    public static string Image(string version = "v0.7.0") {
        return "y4m4/s3www:" + version;
    }

    public S3www(Namespace ns,
        string image,
        string endpoint,
        string accessKey,
        string secretKey,
        string bucket,
        string? ingressHost = null,
        int? lbPort = null,
        string name = NAME
    ) : base(ns, name, "web", Port, image,
        args: new InputList<string> {
            "-endpoint", endpoint,
            "-accessKey", accessKey,
            "-secretKey", secretKey,
            "-bucket", bucket,
            "-address", $"0.0.0.0:{Port}"
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

    public S3www(Namespace ns,
        string image,
        Minio minio,
        string bucket,
        string? ingressHost = null,
        int? lbPort = null,
        string name = NAME
    ) : this(ns, image,
        $"http://{minio.Name}:{Minio.Port}",
        minio.AdminUser, minio.AdminPassword,
        bucket,
        ingressHost: ingressHost,
        lbPort: lbPort,
        name: name
    ) {
    }
}