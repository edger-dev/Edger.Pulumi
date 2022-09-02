namespace Edger.Pulumi.Apps;

using Edger.Pulumi;
using global::Pulumi;

public class Registry : ClusterApp {
    public new const string Name = "registry";
    public const string Image = "registry:2";
    public const int Port = 5000;

    public const string LoadBalancerName = "registry-external";

    public readonly Output<string>? LoadBalancerIP;

    public Registry(Namespace ns, int? lbPort) : base(ns, Name, Image, "api", Port) {
        if (lbPort != null) {
            var lb = ApplyLoadBalancer(LoadBalancerName, lbPort.Value, Port);
            LoadBalancerIP = lb.LoadBalancerIP();
        }
    }
}