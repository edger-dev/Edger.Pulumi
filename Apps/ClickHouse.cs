namespace Edger.Pulumi.Apps;

using Edger.Pulumi;
using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;

public class ClickHouse : StatefulApp {
    public const string NAME = "clickhouse";
    public const int Port = 9000;
    public const int HttpPort = 8123;
    public const int MySqlPort = 9004;
    public const int PgSqlPort = 9005;

    public const string PvcName = "clickhouse-data";
    public const string MountPath = "/bitnami/clickhouse";

    public const string LoadBalancerName = "clickhouse-external";
    public const string HttpLoadBalancerName = "clickhouse-http-external";
    public const string MySqlLoadBalancerName = "clickhouse-mysql-external";
    public const string PgSqlLoadBalancerName = "clickhouse-pgsql-external";

    public readonly Output<string>? LoadBalancerIP;
    public readonly Output<string>? HttpLoadBalancerIP;
    public readonly Output<string>? MySqlLoadBalancerIP;
    public readonly Output<string>? PgSqlLoadBalancerIP;

    public static string Image(string version = "23.3.1") {
        return "bitnami/clickhouse:" + version;
    }

    public static PvcTemplateVolume Volume(
        Namespace ns,
        string requestSize,
        InputMap<string>? labels = null,
        StorageClass? storageClass = null
    ) {
        return new PvcTemplateVolume(ns, PvcName, MountPath, requestSize, labels, storageClass);
    }

    public ClickHouse(Namespace ns,
        string image,
        PvcTemplateVolume pvc,
        string adminPassword,
        string adminUser = "admin",
        int? lbPort = null,
        int? httpLbPort = null,
        int? mySqlLbPort = null,
        int? pgSqlLbPort = null,
        string name = NAME
    ) : base(ns, name, "native",
        Port, "http", HttpPort, "mysql",
        MySqlPort, "pgsql", PgSqlPort,
        image,
        new Volume[] {
            pvc
        },
        env: K8s.ContainerEnv(
            ("CLICKHOUSE_ADMIN_USER", adminUser),
            ("CLICKHOUSE_ADMIN_PASSWORD", adminPassword)
        )
    ) {
        if (lbPort != null) {
            var lb = ApplyLoadBalancer(LoadBalancerName, lbPort.Value, Port);
            LoadBalancerIP = lb.LoadBalancerIP();
        }
        if (httpLbPort != null) {
            var httpLb = ApplyLoadBalancer(HttpLoadBalancerName, httpLbPort.Value, HttpPort);
            HttpLoadBalancerIP = httpLb.LoadBalancerIP();
        }
        if (mySqlLbPort != null) {
            var mySqlLb = ApplyLoadBalancer(MySqlLoadBalancerName, mySqlLbPort.Value, MySqlPort);
            MySqlLoadBalancerIP = mySqlLb.LoadBalancerIP();
        }
        if (pgSqlLbPort != null) {
            var pgSqlLb = ApplyLoadBalancer(PgSqlLoadBalancerName, pgSqlLbPort.Value, PgSqlPort);
            PgSqlLoadBalancerIP = pgSqlLb.LoadBalancerIP();
        }
    }
}