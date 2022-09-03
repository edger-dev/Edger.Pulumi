namespace Edger.Pulumi;

using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;
using global::Pulumi.Kubernetes.Types.Inputs.Apps.V1;
using global::Pulumi.Kubernetes.Types.Inputs.Meta.V1;


using StatefulSet = global::Pulumi.Kubernetes.Apps.V1.StatefulSet;
using global::Pulumi.Kubernetes.Apps.V1;
using global::Pulumi.Kubernetes.Types.Outputs.Apps.V1;

public partial class K8s {
    public static StatefulSetSpecArgs StatefulSetSpec(
        Namespace ns, string name,
        InputMap<string> labels,
        ContainerArgs container,
        InputList<PersistentVolumeClaimArgs> pvcTemplates,
        InputList<VolumeArgs>? podVolumes,
        InputMap<string>? podAnnotations = null,
        int replicas = 1
    ) {
        return new StatefulSetSpecArgs {
            ServiceName = name,
            Selector = new LabelSelectorArgs {
                MatchLabels = labels,
            },
            Replicas = replicas,
            Template = PodTemplateSpec(ns, labels, container,podVolumes, podAnnotations),
            VolumeClaimTemplates = pvcTemplates,
        };
    }

    public static StatefulSetArgs StatefulSet(
        Namespace ns, string name,
        InputMap<string> labels,
        ContainerArgs container,
        InputList<PersistentVolumeClaimArgs> pvcs,
        InputList<VolumeArgs>? podVolumes,
        InputMap<string>? podAnnotations = null,
        int replicas = 1
    ) {
        var metadata = K8s.ObjectMeta(ns, name, labels);
        return new StatefulSetArgs {
            Metadata = metadata,
            Spec = StatefulSetSpec(ns, name, labels, container, pvcs, podVolumes, podAnnotations, replicas),
        };
    }
}

public static class StatefulSetArgsExtension {
    public static StatefulSet Apply(this StatefulSetArgs args, string name) {
        return new StatefulSet(name, args);
    }
}