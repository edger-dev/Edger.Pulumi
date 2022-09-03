namespace Edger.Pulumi.Apps;

using Edger.Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;

public class Grafana : StatefulApp {
    public new const string Name = "grafana";
    public const int Port = 3000;

    public const string PvcName = "grafana-data";
    public const string MountPath = "/var/lib/grafana";


    public static string Image(string version = "9.1.0-ubuntu") {
        return "grafana/grafana:" + version;
    }

    public Grafana(Namespace ns,
        string image,
        Pvc pvc,
        string? ingressHost = null
    ) : base(ns, Name, "ui", Port,
        image,
        GetPvcTemplates(pvc),
        GetVolumeMounts(PvcName, MountPath)
    ) {
        if (ingressHost != null) {
            ApplyHostIngress(ingressHost, Port);
        }
    }
}