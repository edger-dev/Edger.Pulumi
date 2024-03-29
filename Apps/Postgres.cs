namespace Edger.Pulumi.Apps;

using Edger.Pulumi;
using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;

public class Postgres : StatefulApp {
    public const string NAME = "postgres";
    public const int Port = 5432;

    public const string PvcName = "postgres-data";
    public const string MountPath = "/var/lib/postgresql/data";

    public const string LoadBalancerName = "postgres-external";

    public readonly Output<string>? LoadBalancerIP;

    public static string Image(string version = "15.2") {
        return "postgres:" + version;
    }

    public static PvcTemplateVolume Volume(
        Namespace ns,
        string requestSize,
        InputMap<string>? labels = null,
        StorageClass? storageClass = null
    ) {
        return new PvcTemplateVolume(ns, PvcName, MountPath, requestSize, labels, storageClass);
    }

    public readonly string Password;
    public readonly string User;
    public readonly string Db;

    public Postgres(Namespace ns,
        string image,
        PvcTemplateVolume pvc,
        string password,
        string user,
        string db,
        int? lbPort = null,
        string name = NAME
    ) : base(ns, name, "db", Port,
        image,
        new Volume[] {
            pvc
        },
        env: K8s.ContainerEnv(
            ("POSTGRES_PASSWORD", password),
            ("POSTGRES_USER", user),
            ("POSTGRES_DB", db)
        )
    ) {
        Password = password;
        User = user;
        Db = db;
        if (lbPort != null) {
            var lb = ApplyLoadBalancer(LoadBalancerName, lbPort.Value, Port);
            LoadBalancerIP = lb.LoadBalancerIP();
        }
    }

    public string Url() {
        return $"postgresql://{User}:{Password}@{Name}:{Port}/{Db}";
    }


}