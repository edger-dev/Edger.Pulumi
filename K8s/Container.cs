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
    public static ContainerPortArgs ContainerPort(int port) => new ContainerPortArgs {
        ContainerPortValue = port,
    };

    public static ContainerArgs Container(string name, string image, ContainerPortArgs[] ports) => new ContainerArgs {
        Name = name,
        Image = image,
        Ports = ports,
    };

    public static ContainerArgs Container(string name, string image, int[] ports) {
        var containerPorts = ports.Select(p => ContainerPort(p)).ToArray();
        return Container(name, image, containerPorts);
    }
}
