using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MSLA.Server.Security
{
    /// <summary>Access Rights Interface</summary>
    public interface IAccessRight
    {
        /// <summary>The Doc Object Info</summary>
        Base.DocObjectBase DocObjectInfo
        { get; }
        /// <summary>The Company ID</summary>
        long fldCompany_ID
        { get; }
        /// <summary>The Branch ID</summary>
        long fldBranch_ID
        { get; }

        /// <summary>Set the Access Level</summary>
        /// <param name="AccessLevel">The Document Access Level</param>
        void SetAccessLevel(EnAccessLevel AccessLevel);
    }

    /// <summary>Access Rights Interface for Master Doc</summary>
    public interface IARMaster
    {
        /// <summary>The Master Doc Object Info</summary>
        Base.DocMaster DocObjectInfo
        { get; }

        /// <summary>Set the AccessLevel</summary>
        /// <param name="AccessLevel">The Master Access Level</param>
        void SetAccessLevel(EnAccessLevelMaster AccessLevel);

        /// <summary>Set if Delete Is Allowed</summary>
        /// <param name="IsDelAllow">True if delete is allowed</param>
        void SetIsDeleteAllowed(bool IsDelAllow);
    }
}
