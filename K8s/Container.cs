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

    public static ContainerArgs Container(
        string name, string image,
        ContainerPortArgs[] ports,
        EnvVarArgs[]? env = null,
        InputList<string>? args = null, InputList<string>? command = null
    ) {
        if (env != null) {
            if (args != null) {
                if (command != null) {
                    return new ContainerArgs {
                        Name = name,
                        Image = image,
                        Ports = ports,
                        Env = env,
                        Args = args,
                        Command = command,
                    };

                } else {
                    return new ContainerArgs {
                        Name = name,
                        Image = image,
                        Ports = ports,
                        Env = env,
                        Args = args,
                    };
                }
            } else {
                if (command != null) {
                    return new ContainerArgs {
                        Name = name,
                        Image = image,
                        Ports = ports,
                        Env = env,
                        Command = command,
                    };

                } else {
                    return new ContainerArgs {
                        Name = name,
                        Image = image,
                        Ports = ports,
                        Env = env,
                    };
                }
            }
        } else {
            if (args != null) {
                if (command != null) {
                    return new ContainerArgs {
                        Name = name,
                        Image = image,
                        Ports = ports,
                        Args = args,
                        Command = command,
                    };
                } else {
                    return new ContainerArgs {
                        Name = name,
                        Image = image,
                        Ports = ports,
                        Args = args,
                    };
                }

            } else {
                if (command != null) {
                    return new ContainerArgs {
                        Name = name,
                        Image = image,
                        Ports = ports,
                        Command = command,
                    };
                } else {
                    return new ContainerArgs {
                        Name = name,
                        Image = image,
                        Ports = ports,
                    };
                }
            }
        }
    }

    public static ContainerArgs Container(string name, string image, int[] ports, EnvVarArgs[]? env = null) {
        var containerPorts = ports.Select(p => ContainerPort(p)).ToArray();
        return Container(name, image, containerPorts, env);
    }
}
