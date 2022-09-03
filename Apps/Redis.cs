namespace Edger.Pulumi.Apps;

using Edger.Pulumi;
using global::Pulumi;

public class Redis : StatelessApp {
    public new const string Name = "redis";
    public const int Port = 6379;

    public const string LoadBalancerName = "redis-external";

    public readonly Output<string>? LoadBalancerIP;

    public static string Image(string version) {
        return "bitnami/redis:" + version;
    }

    public Redis(Namespace ns,
        string image,
        string password,
        int? lbPort = null
    ) : base(ns, Name, "api", Port, image,
        K8s.ContainerEnv(
            ("REDIS_PASSWORD", password)
        )
    ) {
        if (lbPort != null) {
            var lb = ApplyLoadBalancer(LoadBalancerName, lbPort.Value, Port);
            LoadBalancerIP = lb.LoadBalancerIP();
        }
    }
}