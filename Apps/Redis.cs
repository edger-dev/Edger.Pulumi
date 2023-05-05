namespace Edger.Pulumi.Apps;

using Edger.Pulumi;
using global::Pulumi;

public class Redis : StatelessApp {
    public const string NAME = "redis";
    public const int Port = 6379;

    public const string LoadBalancerName = "redis-external";

    public const string MountedEtcConfigMapName = "redis-etc";
    public const string MountedEtcConfigMountName = "overrides";
    public const string MountedEtConfigMountPath = "/opt/bitnami/redis/mounted-etc";

    public const string OverridesFileName = "overrides.conf";


    public readonly Output<string>? LoadBalancerIP;

    public static string Image(string version = "7.0") {
        return "bitnami/redis:" + version;
    }

    public static Volume GetEtcOverridesVolume(
        Namespace ns,
        string content
    ) {
        return new ConfigMapVolume(
            ns,
            MountedEtcConfigMapName,
            MountedEtConfigMountPath,
            MountedEtcConfigMountName,
            (OverridesFileName, content)
        );
    }

    public Redis(Namespace ns,
        string image,
        string password,
        int? lbPort = null,
        string name = NAME
    ) : base(ns, name, "api", Port, image,
        K8s.ContainerEnv(
            ("REDIS_PASSWORD", password)
        )
    ) {
        if (lbPort != null) {
            var lb = ApplyLoadBalancer(LoadBalancerName, lbPort.Value, Port);
            LoadBalancerIP = lb.LoadBalancerIP();
        }
    }

    public Redis(Namespace ns,
        string image,
        string password,
        Volume[] volumes,
        int? lbPort = null,
        string name = NAME
    ) : base(ns, name, "api", Port, image,
        K8s.ContainerEnv(
            ("REDIS_PASSWORD", password)
        ),
        podVolumes: Volume.FilterVolumes(volumes)
    ) {
        if (lbPort != null) {
            var lb = ApplyLoadBalancer(LoadBalancerName, lbPort.Value, Port);
            LoadBalancerIP = lb.LoadBalancerIP();
        }
    }
}