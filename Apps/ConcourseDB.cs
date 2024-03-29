namespace Edger.Pulumi.Apps;

using System.Text;
using Edger.Pulumi;
using global::Pulumi;
using global::Pulumi.Kubernetes.Types.Inputs.Core.V1;

public class ConcourseDB : Postgres {
    public new const string NAME = "concourse-db";

    public const string DB = "concourse";
    public const string USER = "concourse";

    public ConcourseDB(Namespace ns,
        string image,
        PvcTemplateVolume pvc,
        string password,
        string user = USER,
        string db = DB,
        int? lbPort = null,
        string name = NAME
    ) : base(ns, image, pvc, password, user, db, lbPort:lbPort, name:name
    ) {
    }
}