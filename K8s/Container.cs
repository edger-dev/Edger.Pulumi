namespace Edger.Pulumi;

using System.Linq;
using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;

public partial class K8s {
    public static ContainerPortArgs ContainerPort(int port) => new ContainerPortArgs {
        ContainerPortValue = port,
    };

    public static EnvVarArgs ContainerEnvVar(string name, string val) => new EnvVarArgs {
        Name = name,
        Value = val,
    };

    public static EnvVarArgs[] ContainerEnv(params (string, string)[] args) =>
        args.Select(kv => ContainerEnvVar(kv.Item1, kv.Item2)).ToArray();

    public static VolumeMountArgs ContainerVolumeMount(string name, string mountPath) => new VolumeMountArgs {
        Name = name,
        MountPath = mountPath,
    };

    public static ContainerArgs Container(
        string name, string image,
        ContainerPortArgs[] ports,
        EnvVarArgs[]? env = null,
        InputList<string>? args = null,
        InputList<string>? command = null,
        Input<SecurityContextArgs>? securityContext = null,
        InputList<VolumeMountArgs>? volumeMounts = null
    ) {
        var container = new ContainerArgs {
            Name = name,
            Image = image,
            Ports = ports,
        };
        if (env != null) {
            container.Env = env;
        }
        if (args != null) {
            container.Args = args;
        }
        if (command != null) {
            container.Command = command;
        }
        if (securityContext != null) {
            container.SecurityContext = securityContext;
        }
        if (volumeMounts != null) {
            container.VolumeMounts = volumeMounts;
        }
        return container;
    }

    public static ContainerArgs Container(string name, string image, int[] ports, EnvVarArgs[]? env = null) {
        var containerPorts = ports.Select(p => ContainerPort(p)).ToArray();
        return Container(name, image, containerPorts, env);
    }
}
