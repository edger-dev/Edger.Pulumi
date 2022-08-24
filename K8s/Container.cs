namespace Edger.Pulumi;

using System.Linq;
using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;

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
