namespace Edger.Pulumi.Apps;

using Edger.Pulumi;
using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;

public class PlantUML : StatelessApp {
    public const string NAME = "plantuml";
    public const int Port = 8080;

    public const string LoadBalancerName = "plantuml-external";

    public readonly Output<string>? LoadBalancerIP;

    public static string Image(string version = "jetty") {
        return "plantuml/plantuml-server:" + version;
    }

    public PlantUML(Namespace ns,
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