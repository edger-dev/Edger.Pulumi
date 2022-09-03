namespace Edger.Pulumi.Apps;

using Edger.Pulumi;
using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;

public class MariaDB : StatefulApp {
    public new const string Name = "mariadb";
    public const int Port = 3306;

    public const string PvcName = "mariadb-data";
    public const string MountName = "data";
    public const string MountPath = "/var/lib/mysql";

    public const string LoadBalancerName = "mariadb-external";

    public readonly Output<string>? LoadBalancerIP;

    public static string Image(string version) {
        return "mariadb:" + version;
    }

    private static InputList<PersistentVolumeClaimArgs> GetPvcs(Namespace ns,
        string requestSize,
        StorageClass? storageClass = null
    ) => new InputList<PersistentVolumeClaimArgs> {
        K8s.Pvc(ns, PvcName, requestSize:requestSize, storageClass:storageClass)
    };

    private static InputList<VolumeArgs> GetVolumes() => new InputList<VolumeArgs> {
        K8s.PvcVolume(MountName, PvcName)
    };

    private static InputList<VolumeMountArgs> GetVolumeMounts() => new InputList<VolumeMountArgs> {
        K8s.ContainerVolume(MountName, MountPath)
    };


    public MariaDB(Namespace ns,
        string image,
        string rootPassword,
        string user,
        string password,
        int? lbPort = null,
        string requestSize = "10Gi",
        StorageClass? storageClass = null
    ) : base(ns, Name, "db", Port,
        image, GetPvcs(ns, requestSize, storageClass), GetVolumes(), GetVolumeMounts(),
        env: K8s.ContainerEnv(
            ("MYSQL_ROOT_PASSWORD", rootPassword),
            ("MYSQL_USER", user),
            ("MYSQL_PASSWORD", password)
        ), args: new InputList<string> {
            "mysqld",
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