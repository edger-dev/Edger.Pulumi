namespace Edger.Pulumi.Apps;

using Edger.Pulumi;
using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;

public class Postgres : StatefulApp {
    public new const string Name = "postgres";
    public const int Port = 5432;

    public const string PvcName = "postgres-data";
    public const string MountPath = "/var/lib/postgresql/data";

    public const string LoadBalancerName = "postgres-external";

    public readonly Output<string>? LoadBalancerIP;

    public static string Image(string version) {
        return "postgres:" + version;
    }

    public Postgres(Namespace ns,
        string image,
        PvcTemplateVolume pvc,
        string user,
        string password,
        string db,
        int? lbPort = null
    ) : base(ns, Name, "db", Port,
        image,
        new Volume[] {
            pvc
        },
        env: K8s.ContainerEnv(
            ("POSTGRES_USER", user),
            ("POSTGRES_PASSWORD", password),
            ("POSTGRES_DB", db)
        ), args: new InputList<string> {
            " -c max_connections=1024",
        }
    ) {
        if (lbPort != null) {
            var lb = ApplyLoadBalancer(LoadBalancerName, lbPort.Value, Port);
            LoadBalancerIP = lb.LoadBalancerIP();
        }
    }
}