namespace Edger.Pulumi.Apps;

using Edger.Pulumi;
using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;

public class Minio : StatefulApp {
    public const string NAME = "minio";
    public const int Port = 9000;
    public const int ConsolePort = 9001;

    public const string PvcName = "minio-data";
    public const string MountPath = "/data";

    public const string LoadBalancerName = "minio-external";
    public const string ConsoleLoadBalancerName = "minio-console-external";

    public readonly Output<string>? LoadBalancerIP;
    public readonly Output<string>? ConsoleLoadBalancerIP;

    public static string Image(string version = "latest") {
        return "quay.io/minio/minio:" + version;
    }

    public readonly string AdminUser;
    public readonly string AdminPassword;

    public static PvcTemplateVolume Volume(
        Namespace ns,
        string requestSize,
        InputMap<string>? labels = null,
        StorageClass? storageClass = null
    ) {
        return new PvcTemplateVolume(ns, PvcName, MountPath, requestSize, labels, storageClass);
    }

    public Minio(Namespace ns,
        string image,
        PvcTemplateVolume pvc,
        string serverUrl,
        string browserRedirectUrl,
        string adminPassword,
        string adminUser = "admin",
        string? ingressHost = null,
        int? lbPort = null,
        string? consoleIngressHost = null,
        int? consoleLbPort = null,
        string name = NAME
    ) : base(ns, name, "api", Port, "console", ConsolePort, image,
        new Volume[] {
            pvc,
        },
        env: K8s.ContainerEnv(
            ("MINIO_SERVER_URL", serverUrl)
            ("MINIO_BROWSER_REDIRECT_URL", browserRedirectUrl)
            ("MINIO_ROOT_USER", adminUser),
            ("MINIO_ROOT_PASSWORD", adminPassword)
        ),
        args: new InputList<string> {
            "server",
            MountPath,
            "--console-address",
            $":{ConsolePort}",
        }
    ) {
        AdminUser = adminUser;
        AdminPassword = adminPassword;
        if (ingressHost != null) {
            ApplyHostIngress(ingressHost, Port);
        }
        if (lbPort != null) {
            var lb = ApplyLoadBalancer(LoadBalancerName, lbPort.Value, Port);
            LoadBalancerIP = lb.LoadBalancerIP();
        }
        if (consoleIngressHost != null) {
            ApplyHostIngress(consoleIngressHost, ConsolePort);
        }
        if (consoleLbPort != null) {
            var consoleLb = ApplyLoadBalancer(ConsoleLoadBalancerName, consoleLbPort.Value, ConsolePort);
            ConsoleLoadBalancerIP = consoleLb.LoadBalancerIP();
        }
    }
}
