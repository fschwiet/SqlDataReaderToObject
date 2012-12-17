$testPath = ".\SqlDataReaderToObject.Tests\bin\Debug\SqlDataReaderToObject.Tests.dll"

if (-not (test-path $testPath)) {
	throw "Expected to find tests at $testPath";
}

.\packages\NUnit.Runners.2.6.2\tools\nunit.exe $testPath