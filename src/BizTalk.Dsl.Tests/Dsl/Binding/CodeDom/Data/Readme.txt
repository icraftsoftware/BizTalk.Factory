
ProcessOrchestrationBinding.Designer.cs, from the Be.Stateless.BizTalk.Binding project, is added as link twice
to the Be.Stateless.BizTalk.Dsl.Tests project, first --lexicographically speaking-- as an item of the Compile
ItemGroup, second as an item of EmbeddedResource ItemGroup, that is

<Compile Include="..\BizTalk.Binding\Orchestrations.Dummy\ProcessOrchestrationBinding.Designer.cs">
  <Link>Dsl\Binding\ProcessOrchestrationBinding.Designer.cs</Link>
</Compile>
...
<EmbeddedResource Include="..\BizTalk.Binding\Orchestrations.Dummy\ProcessOrchestrationBinding.Designer.cs">
  <Link>Dsl\Binding\CodeDom\Data\ProcessOrchestrationBinding.Designer.cs</Link>
</EmbeddedResource>

Visual Studio cannot cope with the same item being added twice into different ItemGroups and only displays its
first occurrence. That this is why it does not appear underneath the Data folder.