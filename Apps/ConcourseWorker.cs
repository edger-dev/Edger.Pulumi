namespace Edger.Pulumi.Apps;

using System.Text;
using Edger.Pulumi;
using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;

public class ConcourseWorker : StatefulApp {
    public new const string Name = "concourse-worker";

    public const string KeysMapName = "concourse-worker-keys";
    public const string KeysMountPath = "/concourse-keys";
    public const string KeysMountName = "keys";

    public const string ConfigKey_tsa_host_key_pub = "tsa_host_key.pub";
    public const string ConfigKey_worker_key = "worker_key";

    public const string PvcName = "concourse-worker-data";
    public const string MountPath = "/opt/concourse/worker";

    public static string Image(string version = "7.9.1") {
        return ConcourseWeb.Image(version);
    }

    public static PvcTemplateVolume Volume(
        Namespace ns,
        string requestSize,
        InputMap<string>? labels = null,
        StorageClass? storageClass = null
    ) {
        return new PvcTemplateVolume(ns, PvcName, MountPath, requestSize, labels, storageClass);
    }

    public static ConfigMapVolume GetKeysVolume(
        Namespace ns,
        string tsa_host_key_pub,
        string worker_key
    ) {
        return new ConfigMapVolume(
            ns, KeysMapName,
            KeysMountPath, KeysMountName,
            (ConfigKey_tsa_host_key_pub, tsa_host_key_pub),
            (ConfigKey_worker_key, worker_key)
        );
    }

    public ConcourseWorker(Namespace ns,
        string image,
        PvcTemplateVolume pvc,
        ConfigMapVolume keys,
        string tsaHost = ConcourseWeb.Name,
        string workerDir = MountPath,
        string name = Name
    ) : base(ns, name, image,
        new Volume[] {
            pvc,
            keys
        },
        env: K8s.ContainerEnv(
            ("CONCOURSE_WORK_DIR", workerDir),
            ("CONCOURSE_TSA_HOST", $"{tsaHost}:{ConcourseWeb.TsaPort}")
        ), args: new InputList<string> {
            "worker",
        }, securityContext: K8s.SecurityContext(
            privileged: true,
            allowPrivilegeEscalation: true
        )
    ) {
    }
}