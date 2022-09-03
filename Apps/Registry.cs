namespace Edger.Pulumi.Apps;

using Edger.Pulumi;
using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;

public class Registry : StatefulApp {
    public new const string Name = "registry";
    public const string Image = "registry:2";
    public const int Port = 5000;

    public const string PvcName = "registry-data";
    public const string MountPath = "/var/lib/grafana";

    public const string LoadBalancerName = "registry-external";

    public readonly Output<string>? LoadBalancerIP;

    public Registry(Namespace ns,
        Pvc pvc,
        string? ingressHost = null,
        int? lbPort = null
    ) : base(ns, Name, "api", Port, Image,
        GetPvcTemplates(pvc),
        GetVolumeMounts(PvcName, MountPath)
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