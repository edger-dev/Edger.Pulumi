namespace Edger.Pulumi.Apps;

using System.Text;
using Edger.Pulumi;
using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;

public class Clash : StatefulApp {
    public const string NAME = "clash";
    public const int Port = 1101;
    public const int HttpPort = 1102;
    public const int ControlPort = 1109;

    public const string LoadBalancerName = "clash-external";
    public const string HttpLoadBalancerName = "clash-http-external";
    public const string ControlLoadBalancerName = "clash-control-external";

    public const string ConfigMapName = "clash-config";
    public const string ConfigMountPath = "/etc/clash";
    public const string ConfigMountName = "config";
    public const string ConfigFileName = "config.yaml";

    public readonly Output<string>? LoadBalancerIP;
    public readonly Output<string>? HttpLoadBalancerIP;
    public readonly Output<string>? ControlLoadBalancerIP;

    public static string Image(string version = "v1.14.0") {
        return "dreamacro/clash:" + version;
    }

    public const string CONFIG = @"
socks-port: 1101
port: 1102
allow-lan: true
external-controller: 127.0.0.1:1109
";


    private static Volume GetConfigVolume(
        Namespace ns,
        string config
    ) {
        return new ConfigMapVolume(
            ns, ConfigMapName,
            ConfigMountPath, ConfigMountName,
            (ConfigFileName, config)
        );
    }

    public Clash(Namespace ns,
        string image,
        string config,
        string? ingressHost = null,
        int? lbPort = null,
        int? httpLbPort = null,
        int? controlLbPort = null,
        string name = NAME
    ) : base(ns, name, "socks", Port, "http", HttpPort, "control", ControlPort,
        image,
        new Volume[] {
            GetConfigVolume(ns, config),
        }
        , args: new InputList<string> {
            "-f", $"{ConfigMountPath}/{ConfigFileName}"
        }
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