namespace Edger.Pulumi.Apps;

using Edger.Pulumi;
using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;

public class MariaDB : StatefulApp {
    public new const string Name = "mariadb";
    public const int Port = 3306;

    public const string PvcName = "mariadb-data";
    public const string MountPath = "/var/lib/mysql";

    public const string LoadBalancerName = "mariadb-external";

    public readonly Output<string>? LoadBalancerIP;

    public static string Image(string version) {
        return "mariadb:" + version;
    }

    public MariaDB(Namespace ns,
        string image,
        PvcTemplateVolume pvc,
        string rootPassword,
        string user,
        string password,
        int? lbPort = null
    ) : base(ns, Name, "db", Port,
        image,
        new Volume[] {
            pvc
        },
        env: K8s.ContainerEnv(
            ("MYSQL_ROOT_PASSWORD", rootPassword),
            ("MYSQL_USER", user),
            ("MYSQL_PASSWORD", password)
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