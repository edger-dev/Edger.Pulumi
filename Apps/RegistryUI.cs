namespace Edger.Pulumi.Apps;

using Edger.Pulumi;
using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;

public class RegistryUI : StatelessApp {
    public new const string Name = "registry-ui";
    public const int Port = 80;

    public const string LoadBalancerName = "registry-ui-external";

    public readonly Output<string>? LoadBalancerIP;

    public static string Image(string version = "2.4.1") {
        return "joxit/docker-registry-ui:" + version;
    }

    public RegistryUI(Namespace ns,
        string image,
        string registryUrl,
        bool singleRegistry = true,
        string? ingressHost = null,
        int? lbPort = null,
        string name = Name
    ) : base(ns, name, "web", Port, image,
        env: K8s.ContainerEnv(
            ("REGISTRY_URL", registryUrl),
            ("SINGLE_REGISTRY", singleRegistry ? "true" : "false")
        )
    ) {
        if (ingressHost != null) {
            ApplyHostIngress(ingressHost, Port);
        }
        if (lbPort != null) {
            var lb = ApplyLoadBalancer(LoadBalancerName, lbPort.Value, Port);
            LoadBalancerIP = lb.LoadBalancerIP();
        }
    }
}