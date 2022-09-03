using Edger.Pulumi;

public class Grafana : ClusterApp {
    public new const string Name = "grafana";
    public const string Image = "grafana/grafana:9.1.0-ubuntu";
    public const int Port = 3000;

    public const string IngressHost = "grafana.bigd.local";

    public Grafana(Namespace ns) : base(ns, Name, Image, "ui", Port) {
        var ingress = ApplyHostIngress(IngressHost, Port);
    }
}