namespace Edger.Pulumi.Apps;

using Edger.Pulumi;
using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;

public class MariaDB : StatefulApp {
    public const string NAME = "mariadb";
    public const int Port = 3306;

    public const string PvcName = "mariadb-data";
    public const string MountPath = "/var/lib/mysql";

    public const string LoadBalancerName = "mariadb-external";

    public readonly Output<string>? LoadBalancerIP;

    public static string Image(string version = "10.11") {
        return "mariadb:" + version;
    }

    public static PvcTemplateVolume Volume(
        Namespace ns,
        string requestSize,
        InputMap<string>? labels = null,
        StorageClass? storageClass = null
    ) {
        return new PvcTemplateVolume(ns, PvcName, MountPath, requestSize, labels, storageClass);
    }

    public MariaDB(Namespace ns,
        string image,
        PvcTemplateVolume pvc,
        string rootPassword,
        string user,
        string password,
        int? lbPort = null,
        string name = NAME
    ) : base(ns, name, "db", Port,
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