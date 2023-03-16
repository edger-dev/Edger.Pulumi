namespace Edger.Pulumi.Apps;

using System.Text;
using Edger.Pulumi;
using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;

public class ConcourseDB : Postgres {
    public new const string Name = "concourse-db";

    public const string Db = "concourse";
    public const string User = "concourse";

    public ConcourseDB(Namespace ns,
        string image,
        PvcTemplateVolume pvc,
        string password,
        string user = User,
        string db = Db,
        int? lbPort = null,
        string name = Name
    ) : base(ns, image, pvc, password, user, db, lbPort:lbPort, name:name
    ) {
    }
}