using Edger.Pulumi;

public class Prometheus : ClusterApp {
    public new const string Name = "prometheus";
    public const string Image = "prom/prometheus:v2.38.0";
    public const int Port = 9090;

    public Prometheus(Namespace ns) : base(ns, Name, Image, "api", Port) {
    }
}


