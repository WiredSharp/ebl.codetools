foreach ($folder in @("bin", "obj", "test_project")) {
    Get-ChildItem -Filter $folder -Directory -Recurse | Remove-Item -Recurse
  }
  