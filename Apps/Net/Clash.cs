namespace Edger.Pulumi.Apps;

using System.Text;
using Edger.Pulumi;
using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;

public class Clash : StatelessApp {
    public const string NAME = "clash";
    public const int Port = 7891;
    public const int HttpPort = 7890;
    public const int ControlPort = 9090;

    public const string LoadBalancerName = "clash-external";
    public const string HttpLoadBalancerName = "clash-http-external";
    public const string ControlLoadBalancerName = "clash-control-external";

    public readonly Output<string>? LoadBalancerIP;
    public readonly Output<string>? HttpLoadBalancerIP;
    public readonly Output<string>? ControlLoadBalancerIP;

    public static string Image(string version = "v1.14.0") {
        return "dreamacro/clash:" + version;
    }

    public Clash(Namespace ns,
        string image,
        string? ingressHost = null,
        int? lbPort = null,
        int? httpLbPort = null,
        int? controlLbPort = null,
        string name = NAME
    ) : base(ns, name, "socks", Port, "http", HttpPort, "control", ControlPort,
        image
    ) {
        if (ingressHost != null) {
            ApplyHostIngress(ingressHost, HttpPort);
        }
        if (lbPort != null) {
            var lb = ApplyLoadBalancer(LoadBalancerName, lbPort.Value, Port);
            LoadBalancerIP = lb.LoadBalancerIP();
        }
        if (httpLbPort != null) {
            var httpLb = ApplyLoadBalancer(HttpLoadBalancerName, httpLbPort.Value, HttpPort);
            HttpLoadBalancerIP = httpLb.LoadBalancerIP();
        }
        if (controlLbPort != null) {
            var controlLb = ApplyLoadBalancer(ControlLoadBalancerName, controlLbPort.Value, ControlPort);
            ControlLoadBalancerIP = controlLb.LoadBalancerIP();
        }
    }
}