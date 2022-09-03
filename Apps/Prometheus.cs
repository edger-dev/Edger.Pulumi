namespace Edger.Pulumi.Apps;

using System.Text;
using Edger.Pulumi;
using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;

public class Prometheus : StatefulApp {
    public new const string Name = "prometheus";
    public const int Port = 9090;

    public const string PvcName = "prometheus-data";
    public const string MountPath = "/data";

    public const string ConfigMapName = "prometheus-config";
    public const string ConfigMountName = "config";
    public const string ConfigMountPath = "/etc/prometheus";

    public static string Image(string version = "v2.38.0") {
        return "prom/prometheus:" + version;
    }

    public static string MetricsTarget(string address) {
        return $@"
static_configs:
  - targets: ['{address}']
";
    }

    public static void AppendScrapeConfig(StringBuilder sb, string job_name, string job_text) {
        sb.AppendLine($"  - job_name: '{job_name}'");
        foreach (var line in job_text.Split("\n")) {
            sb.AppendLine($"    {line}");
        }
    }

    public static string Config(
        int intervalSeconds,
        params (string, string)[] scrape_configs
    ) {
        var sb = new StringBuilder();
        sb.AppendLine("global:");
        sb.AppendLine($"  scrape_interval: {intervalSeconds}s");
        sb.AppendLine($"  evaluation_interval: {intervalSeconds}s");
        sb.AppendLine("scrape_configs:");
        AppendScrapeConfig(sb, "prometheus", MetricsTarget("localhost:9090"));
        foreach (var scrape in scrape_configs) {
            AppendScrapeConfig(sb, scrape.Item1, scrape.Item2);
        }
        return sb.ToString();
    }

    private static InputList<VolumeArgs> GetVolumes(
        Namespace ns,
        string config
    ) {
        var configData = new InputMap<string> {
            { "prometheus.yaml", config }
        };
        var configMap = K8s.ConfigMap(ns, ConfigMapName, configData, immutable:true);
        return new InputList<VolumeArgs> {
            K8s.ConfigMapVolume(ConfigMountName, configMap.Apply(ConfigMapName))
        };
    }

    public Prometheus(Namespace ns,
        string image,
        Pvc pvc,
        string config,
        string? ingressHost = null
    ) : base(ns, Name, "api", Port,
        image,
        GetPvcTemplates(pvc),
        GetVolumeMounts(PvcName, MountPath, ConfigMountName, ConfigMountPath),
        GetVolumes(ns, config),
        args: new InputList<string> {
            "--config.file=/etc/prometheus/prometheus.yaml",
            "--storage.tsdb.path=/data",
        }
    ) {
        if (ingressHost != null) {
            ApplyHostIngress(ingressHost, Port);
        }
    }
}


