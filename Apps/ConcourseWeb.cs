namespace Edger.Pulumi.Apps;

using System.Text;
using Edger.Pulumi;
using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;

public class ConcourseWeb : StatefulApp {
    public new const string Name = "concourse-web";
    public const int Port = 8080;
    public const int TsaPort = 2222;

    public const string KeysMapName = "concourse-web-keys";
    public const string KeysMountPath = "/concourse-keys";
    public const string KeysMountName = "keys";

    public const string ConfigKey_session_signing_key = "session_signing_key";
    public const string ConfigKey_tsa_host_key = "tsa_host_key";
    public const string ConfigKey_authorized_worker_keys = "authorized_worker_keys";

    public const string LoadBalancerName = "concourse-web-external";
    public const string TsaLoadBalancerName = "concourse-tsa-external";

    public readonly Output<string>? LoadBalancerIP;
    public readonly Output<string>? TsaLoadBalancerIP;

    public static string Image(string version = "7.9.1") {
        return "concourse/concourse:" + version;
    }

    public static ConfigMapVolume GetKeysVolume(
        Namespace ns,
        string session_signing_key,
        string tsa_host_key,
        string authorized_worker_keys
    ) {
        return new ConfigMapVolume(
            ns, KeysMapName,
            KeysMountPath, KeysMountName,
            (ConfigKey_session_signing_key, session_signing_key),
            (ConfigKey_tsa_host_key, tsa_host_key),
            (ConfigKey_authorized_worker_keys, authorized_worker_keys)
        );
    }

    public ConcourseWeb(Namespace ns,
        string image,
        ConfigMapVolume keys,
        string dbPassword,
        string externalUrl,
        string adminPassword,
        string adminUser = "admin",
        string dbUser = ConcourseDB.User,
        string dbDatabase = ConcourseDB.Db,
        string dbHost = ConcourseDB.Name,
        int dbPort = ConcourseDB.Port,
        string? ingressHost = null,
        int? lbPort = null,
        int? tsaLbPort = null,
        string name = Name
    ) : base(ns, name, "web", Port, "tsa", TsaPort, image,
        new Volume[] {
            keys
        },
        env: K8s.ContainerEnv(
            ("CONCOURSE_EXTERNAL_URL", externalUrl),
            ("CONCOURSE_ADD_LOCAL_USER", $"{adminUser}:{adminPassword}"),
            ("CONCOURSE_MAIN_TEAM_LOCAL_USER", adminUser),
            ("CONCOURSE_POSTGRES_HOST", dbHost),
            ("CONCOURSE_POSTGRES_PORT", dbPort.ToString()),
            ("CONCOURSE_POSTGRES_PASSWORD", dbPassword),
            ("CONCOURSE_POSTGRES_USER", dbUser),
            ("CONCOURSE_POSTGRES_DATABASE", dbDatabase)
        ), args: new InputList<string> {
            "web",
        }
    ) {
        if (ingressHost != null) {
            ApplyHostIngress(ingressHost, Port);
        }
        if (lbPort != null) {
            var lb = ApplyLoadBalancer(LoadBalancerName, lbPort.Value, Port);
            LoadBalancerIP = lb.LoadBalancerIP();
        }
        if (tsaLbPort != null) {
            var tsaLb = ApplyLoadBalancer(TsaLoadBalancerName, tsaLbPort.Value, TsaPort);
            TsaLoadBalancerIP = tsaLb.LoadBalancerIP();
        }
    }
}