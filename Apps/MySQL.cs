namespace Edger.Pulumi.Apps;

using Edger.Pulumi;
using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;

public class MySQL : StatefulApp {
    public const string NAME = "mysql";
    public const int Port = 3306;

    public const string PvcName = "mysql-data";
    public const string MountPath = "/var/lib/mysql";

    public const string LoadBalancerName = "mysql-external";

    public readonly Output<string>? LoadBalancerIP;

    public static string Image(string version = "8.0") {
        return "mysql:" + version;
    }

    public static PvcTemplateVolume Volume(
        Namespace ns,
        string requestSize,
        InputMap<string>? labels = null,
        StorageClass? storageClass = null
    ) {
        return new PvcTemplateVolume(ns, PvcName, MountPath, requestSize, labels, storageClass);
    }

    public record Args(
        string RootPassword,
        string Database,
        string User,
        string Password
    );

    public MySQL(Namespace ns,
        string image,
        PvcTemplateVolume pvc,
        Args args,
        int? lbPort = null,
        string name = NAME
    ) : base(ns, name, "db", Port,
        image,
        new Volume[] {
            pvc
        },
        env: K8s.ContainerEnv(
            ("MYSQL_ROOT_PASSWORD", args.RootPassword),
            ("MYSQL_DATABASE", args.Database),
            ("MYSQL_USER", args.User),
            ("MYSQL_PASSWORD", args.Password)
        ), args: new InputList<string> {
            "--disable_log_bin",
            "--max_allowed_packet=512000000",
            "--max_connections=1024"
        }
    ) {
        if (lbPort != null) {
            var lb = ApplyLoadBalancer(LoadBalancerName, lbPort.Value, Port);
            LoadBalancerIP = lb.LoadBalancerIP();
        }
    }
}