
using UnityEditor;
using UnityEditorInternal;
using System.Text;
using System.Diagnostics;

[InitializeOnLoad]
static class GitBranchIndicator
{
    static GitBranchIndicator()
	{
        EditorApplication.update += Update;
    }
    static void Update()
    {
        var isApplicationActive = InternalEditorUtility.isApplicationActive;
        
        if( m_IsFocused == false && isApplicationActive != false)
        {
			m_IsFocused = true;
            OnFocused();
        }
        else if( m_IsFocused != false && isApplicationActive == false)
        {
			m_IsFocused = false;
            OnUnfocused();
        }
    }
    static void OnFocused()
    {
        EditorApplication.updateMainWindowTitle += OnUpdateMainWindowTitle;
        EditorApplication.UpdateMainWindowTitle();
        EditorApplication.updateMainWindowTitle -= OnUpdateMainWindowTitle;
    }
    static void OnUnfocused()
    {
    }
    static void OnUpdateMainWindowTitle( ApplicationTitleDescriptor descriptor)
    {
        descriptor.title = string.Format( "[{1} - {2}] {0}", 
            descriptor.title, 
            Git( "symbolic-ref --short HEAD"),
            Git( "rev-parse --short HEAD"));
    }
    static string Git( string arguments)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName               = "git",
            Arguments              = arguments,
            CreateNoWindow         = true,
            RedirectStandardError  = true,
            RedirectStandardOutput = true,
            StandardErrorEncoding  = Encoding.UTF8,
            StandardOutputEncoding = Encoding.UTF8,
            UseShellExecute        = false,
        };
        using( var process = Process.Start( startInfo))
        {
            process.WaitForExit();
            return process.StandardOutput.ReadToEnd().Trim();
        }
    }
    
    static bool m_IsFocused;
}
