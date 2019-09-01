$manifest = @{
    Path              = '.\dotnetcli-projects\dotnetcli-projects.psd1'
    RootModule        = 'dotnetcli-projects.psm1' 
    Author            = 'zericco'
}
New-ModuleManifest @manifest