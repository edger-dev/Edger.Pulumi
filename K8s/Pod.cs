namespace Edger.Pulumi;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using global::Pulumi;
using global::Pulumi.Kubernetes.Core.V1;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;
using global::Pulumi.Kubernetes.Types.Inputs.Apps.V1;
using global::Pulumi.Kubernetes.Types.Inputs.Meta.V1;
using global::Pulumi.Kubernetes.Types.Inputs.ApiExtensions.V1Beta1;

public partial class K8s {
    public static PodSpecArgs PodSpec(ContainerArgs container) => new PodSpecArgs {
        Containers = { container },
    };

    public static PodTemplateSpecArgs PodTemplateSpec(Namespace ns, InputMap<string> labels, ContainerArgs container) => new PodTemplateSpecArgs {
        Metadata = new ObjectMetaArgs {
            Namespace = ns.Value,
            Labels = labels,
        },
        Spec = PodSpec(container),
    };
}