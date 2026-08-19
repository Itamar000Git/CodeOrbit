using UnityEngine;

public class LinkOpener : MonoBehaviour
{
    public void OpenResume()
    {
        Application.OpenURL("https://drive.google.com/file/d/1bWZKTmA7IWoQgwauFTJc9gQcSwzITgsA/view?usp=sharing");
    }
     public void Linkedin()
    {
        Application.OpenURL("https://www.linkedin.com/feed/");
    }
     public void Github()
    {
        Application.OpenURL("https://github.com/Itamar000Git");
    }
}