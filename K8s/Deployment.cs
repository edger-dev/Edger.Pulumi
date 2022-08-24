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


using Deployment = global::Pulumi.Kubernetes.Apps.V1.Deployment;

public partial class K8s {
    public static InputMap<string> AppLabels(string name) {
        return new InputMap<string> {
            { "app", name },
        };
    }

    public static ObjectMetaArgs AppMeta(Namespace ns, string name) => new ObjectMetaArgs {
        Namespace = ns.Value,
        Name = name,
        Labels = AppLabels(name),
    };

    public static DeploymentSpecArgs DeploymentSpec(
            Namespace ns, string name,
            ContainerArgs container,
            int replicas = 1) {
        var labels = AppLabels(name);
        return new DeploymentSpecArgs {
            Selector = new LabelSelectorArgs {
                MatchLabels = labels,
            },
            Replicas = replicas,
            Template = PodTemplateSpec(ns, labels, container),
        };
    }

    public static DeploymentArgs Deployment(Namespace ns, string name, ContainerArgs container, int replicas = 1) {
        var metadata = AppMeta(ns, name);
        return new DeploymentArgs {
            Metadata = metadata,
            Spec = DeploymentSpec(ns, name, container, replicas),
        };
    }
}

public static class DeploymentArgsExtension {
    public static Deployment Apply(this DeploymentArgs args, string name) {
        return new Deployment(name, args);
    }
}