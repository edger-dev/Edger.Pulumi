namespace Edger.Pulumi.Apps;

using Edger.Pulumi;
using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;

public class ClashUI : StatelessApp {
    public const string NAME = "clash-ui";
    public const int Port = 80;

    public const string LoadBalancerName = "clash-ui-external";

    public readonly Output<string>? LoadBalancerIP;

    public static string Image(string version = "v0.3.8") {
        return "haishanh/yacd:" + version;
    }

    public ClashUI(Namespace ns,
        string image,
        string? ingressHost = null,
        int? lbPort = null,
        string name = NAME
    ) : base(ns, name, "web", Port, image
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