﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3603
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SIL.Pa.AddOn.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SIL.Pa.AddOn.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        internal static System.Drawing.Icon kicoItemInformation {
            get {
                object obj = ResourceManager.GetObject("kicoItemInformation", resourceCulture);
                return ((System.Drawing.Icon)(obj));
            }
        }
        
        internal static System.Drawing.Bitmap kimidRequestDlgImage {
            get {
                object obj = ResourceManager.GetObject("kimidRequestDlgImage", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        internal static System.Drawing.Bitmap kimidSendFeedback {
            get {
                object obj = ResourceManager.GetObject("kimidSendFeedback", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to PaFeedback@sil.org.
        /// </summary>
        internal static string kstidFeedbackMailAddress {
            get {
                return ResourceManager.GetString("kstidFeedbackMailAddress", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &amp;Send Feedback....
        /// </summary>
        internal static string kstidFeedbackMenuText {
            get {
                return ResourceManager.GetString("kstidFeedbackMenuText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Thank you for trying Phonology Assistant.\r\n\r\nIn order to determine future Phonology Assistant development, spending a few minutes to give your comments and suggestions will be helpful. This information will be placed in an e-mail and sent to the SIL software developers the next time you send e-mail. There is no need to be connected to the internet now and you will have a chance to view the e-mail before it is sent.\r\n\r\nIf you prefer not to provide feedback at this time, but would like to do so later, [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string kstidFeedbackRequestMsg {
            get {
                return ResourceManager.GetString("kstidFeedbackRequestMsg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to One or more items have not been rated. Do you want to send anyway?.
        /// </summary>
        internal static string kstidMissingRatingMsg {
            get {
                return ResourceManager.GetString("kstidMissingRatingMsg", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Description:.
        /// </summary>
        internal static string kstidSurveyItemInfoTitle {
            get {
                return ResourceManager.GetString("kstidSurveyItemInfoTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}. {1}.
        /// </summary>
        internal static string kstidSurveyItemTextFormat {
            get {
                return ResourceManager.GetString("kstidSurveyItemTextFormat", resourceCulture);
            }
        }
    }
}
