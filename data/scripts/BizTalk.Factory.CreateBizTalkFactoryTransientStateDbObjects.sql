/*
Copyright © 2012 - 2017 François Chabot, Yves Dierick

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

USE [BizTalkFactoryTransientStateDb]
GO

/*
BBBBBBBBBBBBBBBBB                             tttt                             hhhhhhh               iiii                                       
B::::::::::::::::B                         ttt:::t                             h:::::h              i::::i                                      
B::::::BBBBBB:::::B                        t:::::t                             h:::::h               iiii                                       
BB:::::B     B:::::B                       t:::::t                             h:::::h                                                          
  B::::B     B:::::B  aaaaaaaaaaaaa  ttttttt:::::ttttttt        cccccccccccccccch::::h hhhhh       iiiiiiinnnn  nnnnnnnn       ggggggggg   ggggg
  B::::B     B:::::B  a::::::::::::a t:::::::::::::::::t      cc:::::::::::::::ch::::hh:::::hhh    i:::::in:::nn::::::::nn    g:::::::::ggg::::g
  B::::BBBBBB:::::B   aaaaaaaaa:::::at:::::::::::::::::t     c:::::::::::::::::ch::::::::::::::hh   i::::in::::::::::::::nn  g:::::::::::::::::g
  B:::::::::::::BB             a::::atttttt:::::::tttttt    c:::::::cccccc:::::ch:::::::hhh::::::h  i::::inn:::::::::::::::ng::::::ggggg::::::gg
  B::::BBBBBB:::::B     aaaaaaa:::::a      t:::::t          c::::::c     ccccccch::::::h   h::::::h i::::i  n:::::nnnn:::::ng:::::g     g:::::g 
  B::::B     B:::::B  aa::::::::::::a      t:::::t          c:::::c             h:::::h     h:::::h i::::i  n::::n    n::::ng:::::g     g:::::g 
  B::::B     B:::::B a::::aaaa::::::a      t:::::t          c:::::c             h:::::h     h:::::h i::::i  n::::n    n::::ng:::::g     g:::::g 
  B::::B     B:::::Ba::::a    a:::::a      t:::::t    ttttttc::::::c     ccccccch:::::h     h:::::h i::::i  n::::n    n::::ng::::::g    g:::::g 
BB:::::BBBBBB::::::Ba::::a    a:::::a      t::::::tttt:::::tc:::::::cccccc:::::ch:::::h     h:::::hi::::::i n::::n    n::::ng:::::::ggggg:::::g 
B:::::::::::::::::B a:::::aaaa::::::a      tt::::::::::::::t c:::::::::::::::::ch:::::h     h:::::hi::::::i n::::n    n::::n g::::::::::::::::g 
B::::::::::::::::B   a::::::::::aa:::a       tt:::::::::::tt  cc:::::::::::::::ch:::::h     h:::::hi::::::i n::::n    n::::n  gg::::::::::::::g 
BBBBBBBBBBBBBBBBB     aaaaaaaaaa  aaaa         ttttttttttt      cccccccccccccccchhhhhhh     hhhhhhhiiiiiiii nnnnnn    nnnnnn    gggggggg::::::g 
                                                                                                                                        g:::::g 
                                                                                                                            gggggg      g:::::g 
                                                                                                                            g:::::gg   gg:::::g 
                                                                                                                             g::::::ggg:::::::g 
                                                                                                                              gg:::::::::::::g  
                                                                                                                                ggg::::::ggg    
                                                                                                                                   gggggg       

http://patorjk.com/software/taag/#p=display&f=Doh&t=Batching
*/

IF  EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_batch_Parts_Partition]') AND parent_object_id = OBJECT_ID(N'[dbo].[batch_Parts]'))
Begin
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_batch_Parts_Partition]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[batch_Parts] DROP CONSTRAINT [DF_batch_Parts_Partition]
END


End
GO
IF  EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_batch_Parts_Timestamp]') AND parent_object_id = OBJECT_ID(N'[dbo].[batch_Parts]'))
Begin
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_batch_Parts_Timestamp]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[batch_Parts] DROP CONSTRAINT [DF_batch_Parts_Timestamp]
END


End
GO
IF  EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_batch_QueuedControlledReleases_Partition]') AND parent_object_id = OBJECT_ID(N'[dbo].[batch_QueuedControlledReleases]'))
Begin
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_batch_QueuedControlledReleases_Partition]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[batch_QueuedControlledReleases] DROP CONSTRAINT [DF_batch_QueuedControlledReleases_Partition]
END


End
GO
IF  EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_batch_QueuedControlledReleases_Timestamp]') AND parent_object_id = OBJECT_ID(N'[dbo].[batch_QueuedControlledReleases]'))
Begin
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_batch_QueuedControlledReleases_Timestamp]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[batch_QueuedControlledReleases] DROP CONSTRAINT [DF_batch_QueuedControlledReleases_Timestamp]
END


End
GO
IF  EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_batch_ReleasePolicyDefinitions_Partition]') AND parent_object_id = OBJECT_ID(N'[dbo].[batch_ReleasePolicyDefinitions]'))
Begin
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_batch_ReleasePolicyDefinitions_Partition]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[batch_ReleasePolicyDefinitions] DROP CONSTRAINT [DF_batch_ReleasePolicyDefinitions_Partition]
END


End
GO
IF  EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_batch_ReleasePolicyDefinitions_EnforceItemCountLimitOnRelease]') AND parent_object_id = OBJECT_ID(N'[dbo].[batch_ReleasePolicyDefinitions]'))
Begin
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_batch_ReleasePolicyDefinitions_EnforceItemCountLimitOnRelease]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[batch_ReleasePolicyDefinitions] DROP CONSTRAINT [DF_batch_ReleasePolicyDefinitions_EnforceItemCountLimitOnRelease]
END


End
GO
/****** Object:  ForeignKey [FK_batch_Parts_batch_Envelopes]    Script Date: 10/11/2013 12:36:31 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_batch_Parts_batch_Envelopes]') AND parent_object_id = OBJECT_ID(N'[dbo].[batch_Parts]'))
ALTER TABLE [dbo].[batch_Parts] DROP CONSTRAINT [FK_batch_Parts_batch_Envelopes]
GO
/****** Object:  ForeignKey [FK_batch_QueuedControlledReleases_batch_Envelopes]    Script Date: 10/11/2013 12:36:31 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_batch_QueuedControlledReleases_batch_Envelopes]') AND parent_object_id = OBJECT_ID(N'[dbo].[batch_QueuedControlledReleases]'))
ALTER TABLE [dbo].[batch_QueuedControlledReleases] DROP CONSTRAINT [FK_batch_QueuedControlledReleases_batch_Envelopes]
GO
/****** Object:  ForeignKey [FK_batch_ReleasePolicyDefinitions_batch_Envelopes]    Script Date: 10/11/2013 12:36:31 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_batch_ReleasePolicyDefinitions_batch_Envelopes]') AND parent_object_id = OBJECT_ID(N'[dbo].[batch_ReleasePolicyDefinitions]'))
ALTER TABLE [dbo].[batch_ReleasePolicyDefinitions] DROP CONSTRAINT [FK_batch_ReleasePolicyDefinitions_batch_Envelopes]
GO
/****** Object:  Table [dbo].[batch_Parts]    Script Date: 10/11/2013 12:36:31 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[batch_Parts]') AND type in (N'U'))
DROP TABLE [dbo].[batch_Parts]
GO
/****** Object:  Table [dbo].[batch_QueuedControlledReleases]    Script Date: 10/11/2013 12:36:31 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[batch_QueuedControlledReleases]') AND type in (N'U'))
DROP TABLE [dbo].[batch_QueuedControlledReleases]
GO
/****** Object:  Table [dbo].[batch_ReleasePolicyDefinitions]    Script Date: 10/11/2013 12:36:31 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[batch_ReleasePolicyDefinitions]') AND type in (N'U'))
DROP TABLE [dbo].[batch_ReleasePolicyDefinitions]
GO
/****** Object:  Table [dbo].[batch_Envelopes]    Script Date: 10/11/2013 12:36:31 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[batch_Envelopes]') AND type in (N'U'))
DROP TABLE [dbo].[batch_Envelopes]
GO
/****** Object:  Table [dbo].[batch_Envelopes]    Script Date: 10/11/2013 12:36:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[batch_Envelopes]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[batch_Envelopes](
   [Id] [int] IDENTITY(1,1) NOT NULL,
   [EnvelopeSpecName] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_batch_Envelopes] PRIMARY KEY CLUSTERED 
(
   [Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[batch_Envelopes]') AND name = N'UX_batch_Envelopes_EnvelopeSpecName')
CREATE UNIQUE NONCLUSTERED INDEX [UX_batch_Envelopes_EnvelopeSpecName] ON [dbo].[batch_Envelopes] 
(
   [EnvelopeSpecName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[batch_ReleasePolicyDefinitions]    Script Date: 12/10/2013 14:03:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[batch_ReleasePolicyDefinitions]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[batch_ReleasePolicyDefinitions](
   [EnvelopeId] [int] NOT NULL,
   [Partition] [nvarchar](128) NOT NULL,
   [Enabled] [bit] NOT NULL,
   [ReleaseOnElapsedTimeOut] [int] NULL,
   [ReleaseOnIdleTimeOut] [int] NULL,
   [ReleaseOnItemCount] [int] NULL,
   [EnforceItemCountLimitOnRelease] [bit] NOT NULL,
 CONSTRAINT [PK_batch_ReleasePolicyDefinitions] PRIMARY KEY CLUSTERED 
(
   [EnvelopeId] ASC,
   [Partition] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'batch_ReleasePolicyDefinitions', N'COLUMN',N'ReleaseOnElapsedTimeOut'))
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'For a given batch (i.e. EnvelopeSpecName and Partition), the maximum amount of time (in minutes) that can elapse, since the very first accumulated batch item, before the batch is automatically released. It also ensures that any batch will eventually be released independently of the sliding nature of the ReleaseOnIdleTimeOut policy.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'batch_ReleasePolicyDefinitions', @level2type=N'COLUMN',@level2name=N'ReleaseOnElapsedTimeOut'
GO
IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'batch_ReleasePolicyDefinitions', N'COLUMN',N'ReleaseOnIdleTimeOut'))
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'For a given batch (i.e. EnvelopeSpecName and Partition), the maximum amount of time (in minutes) that can elapse, since the last accumulated batch item, before the batch is automatically released.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'batch_ReleasePolicyDefinitions', @level2type=N'COLUMN',@level2name=N'ReleaseOnIdleTimeOut'
GO
IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'batch_ReleasePolicyDefinitions', N'COLUMN',N'ReleaseOnItemCount'))
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'For a given batch (i.e. EnvelopeSpecName and Partition), the minimum number of items that can be accumulated before the batch is automatically released.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'batch_ReleasePolicyDefinitions', @level2type=N'COLUMN',@level2name=N'ReleaseOnItemCount'
GO
IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'batch_ReleasePolicyDefinitions', N'COLUMN',N'EnforceItemCountLimitOnRelease'))
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'For a given batch (i.e. EnvelopeSpecName and Partition), it determines whether the ReleaseOnItemCount property value is also used to enforce a maximum size limit on the number of items (i.e. batch parts) that can be released together in a single batch.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'batch_ReleasePolicyDefinitions', @level2type=N'COLUMN',@level2name=N'EnforceItemCountLimitOnRelease'
GO
/****** Object:  Table [dbo].[batch_QueuedControlledReleases]    Script Date: 10/11/2013 12:36:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[batch_QueuedControlledReleases]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[batch_QueuedControlledReleases](
   [EnvelopeId] [int] NOT NULL,
   [Partition] [nvarchar](128) NOT NULL,
   [ProcessActivityId] [nvarchar](32) NULL,
   [Timestamp] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_batch_QueuedControlledReleases] PRIMARY KEY CLUSTERED 
(
   [EnvelopeId] ASC,
   [Partition] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[batch_Parts]    Script Date: 10/11/2013 12:36:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[batch_Parts]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[batch_Parts](
   [Id] [int] IDENTITY(1,1) NOT NULL,
   [EnvelopeId] [int] NOT NULL,
   [MessagingStepActivityId] [nvarchar](32) NULL,
   [Partition] [nvarchar](128) NOT NULL,
   [Data] [nvarchar](max) NOT NULL,
   [Timestamp] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_batch_Parts] PRIMARY KEY CLUSTERED 
(
   [Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[batch_Parts]') AND name = N'IX_batch_Parts_EnvelopeId')
CREATE NONCLUSTERED INDEX [IX_batch_Parts_EnvelopeId] ON [dbo].[batch_Parts] 
(
   [EnvelopeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[batch_Parts]') AND name = N'IX_batch_Parts_Partition')
CREATE NONCLUSTERED INDEX [IX_batch_Parts_Partition] ON [dbo].[batch_Parts] 
(
   [Partition] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[batch_Parts]') AND name = N'UX_batch_Parts_MessagingStepActivityId')
CREATE UNIQUE NONCLUSTERED INDEX [UX_batch_Parts_MessagingStepActivityId] ON [dbo].[batch_Parts] 
(
   [MessagingStepActivityId] ASC
)
WHERE ([MessagingStepActivityId] IS NOT NULL)
WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Default [DF_batch_Parts_Partition]    Script Date: 10/11/2013 12:36:31 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_batch_Parts_Partition]') AND parent_object_id = OBJECT_ID(N'[dbo].[batch_Parts]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_batch_Parts_Partition]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[batch_Parts] ADD  CONSTRAINT [DF_batch_Parts_Partition]  DEFAULT ('0') FOR [Partition]
END


End
GO
/****** Object:  Default [DF_batch_Parts_Timestamp]    Script Date: 10/11/2013 12:36:31 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_batch_Parts_Timestamp]') AND parent_object_id = OBJECT_ID(N'[dbo].[batch_Parts]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_batch_Parts_Timestamp]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[batch_Parts] ADD  CONSTRAINT [DF_batch_Parts_Timestamp]  DEFAULT (sysutcdatetime()) FOR [Timestamp]
END


End
GO
/****** Object:  Default [DF_batch_QueuedControlledReleases_Partition]    Script Date: 10/11/2013 12:36:31 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_batch_QueuedControlledReleases_Partition]') AND parent_object_id = OBJECT_ID(N'[dbo].[batch_QueuedControlledReleases]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_batch_QueuedControlledReleases_Partition]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[batch_QueuedControlledReleases] ADD  CONSTRAINT [DF_batch_QueuedControlledReleases_Partition]  DEFAULT ('0') FOR [Partition]
END


End
GO
/****** Object:  Default [DF_batch_QueuedControlledReleases_Timestamp]    Script Date: 10/11/2013 12:36:31 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_batch_QueuedControlledReleases_Timestamp]') AND parent_object_id = OBJECT_ID(N'[dbo].[batch_QueuedControlledReleases]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_batch_QueuedControlledReleases_Timestamp]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[batch_QueuedControlledReleases] ADD  CONSTRAINT [DF_batch_QueuedControlledReleases_Timestamp]  DEFAULT (sysutcdatetime()) FOR [Timestamp]
END


End
GO
/****** Object:  Default [DF_batch_ReleasePolicyDefinitions_Partition]    Script Date: 10/11/2013 12:36:31 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_batch_ReleasePolicyDefinitions_Partition]') AND parent_object_id = OBJECT_ID(N'[dbo].[batch_ReleasePolicyDefinitions]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_batch_ReleasePolicyDefinitions_Partition]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[batch_ReleasePolicyDefinitions] ADD  CONSTRAINT [DF_batch_ReleasePolicyDefinitions_Partition]  DEFAULT ('0') FOR [Partition]
END


End
GO
/****** Object:  Default [DF_batch_ReleasePolicyDefinitions_EnforceItemCountLimitOnRelease]    Script Date: 12/10/2013 14:03:03 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_batch_ReleasePolicyDefinitions_EnforceItemCountLimitOnRelease]') AND parent_object_id = OBJECT_ID(N'[dbo].[batch_ReleasePolicyDefinitions]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_batch_ReleasePolicyDefinitions_EnforceItemCountLimitOnRelease]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[batch_ReleasePolicyDefinitions] ADD  CONSTRAINT [DF_batch_ReleasePolicyDefinitions_EnforceItemCountLimitOnRelease]  DEFAULT ((0)) FOR [EnforceItemCountLimitOnRelease]
END


End
GO
/****** Object:  ForeignKey [FK_batch_Parts_batch_Envelopes]    Script Date: 10/11/2013 12:36:31 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_batch_Parts_batch_Envelopes]') AND parent_object_id = OBJECT_ID(N'[dbo].[batch_Parts]'))
ALTER TABLE [dbo].[batch_Parts]  WITH CHECK ADD  CONSTRAINT [FK_batch_Parts_batch_Envelopes] FOREIGN KEY([EnvelopeId])
REFERENCES [dbo].[batch_Envelopes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_batch_Parts_batch_Envelopes]') AND parent_object_id = OBJECT_ID(N'[dbo].[batch_Parts]'))
ALTER TABLE [dbo].[batch_Parts] CHECK CONSTRAINT [FK_batch_Parts_batch_Envelopes]
GO
/****** Object:  ForeignKey [FK_batch_QueuedControlledReleases_batch_Envelopes]    Script Date: 10/11/2013 12:36:31 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_batch_QueuedControlledReleases_batch_Envelopes]') AND parent_object_id = OBJECT_ID(N'[dbo].[batch_QueuedControlledReleases]'))
ALTER TABLE [dbo].[batch_QueuedControlledReleases]  WITH CHECK ADD  CONSTRAINT [FK_batch_QueuedControlledReleases_batch_Envelopes] FOREIGN KEY([EnvelopeId])
REFERENCES [dbo].[batch_Envelopes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_batch_QueuedControlledReleases_batch_Envelopes]') AND parent_object_id = OBJECT_ID(N'[dbo].[batch_QueuedControlledReleases]'))
ALTER TABLE [dbo].[batch_QueuedControlledReleases] CHECK CONSTRAINT [FK_batch_QueuedControlledReleases_batch_Envelopes]
GO
/****** Object:  ForeignKey [FK_batch_ReleasePolicyDefinitions_batch_Envelopes]    Script Date: 10/11/2013 12:36:31 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_batch_ReleasePolicyDefinitions_batch_Envelopes]') AND parent_object_id = OBJECT_ID(N'[dbo].[batch_ReleasePolicyDefinitions]'))
ALTER TABLE [dbo].[batch_ReleasePolicyDefinitions]  WITH CHECK ADD  CONSTRAINT [FK_batch_ReleasePolicyDefinitions_batch_Envelopes] FOREIGN KEY([EnvelopeId])
REFERENCES [dbo].[batch_Envelopes] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_batch_ReleasePolicyDefinitions_batch_Envelopes]') AND parent_object_id = OBJECT_ID(N'[dbo].[batch_ReleasePolicyDefinitions]'))
ALTER TABLE [dbo].[batch_ReleasePolicyDefinitions] CHECK CONSTRAINT [FK_batch_ReleasePolicyDefinitions_batch_Envelopes]
GO

/****** Object:  View [dbo].[vw_batch_AvailableBatches]    Script Date: 10/14/2013 10:20:01 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vw_batch_AvailableBatches]'))
DROP VIEW [dbo].[vw_batch_AvailableBatches]
GO

/****** Object:  View [dbo].[vw_batch_NextAvailableBatch]    Script Date: 10/15/2013 20:44:37 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vw_batch_NextAvailableBatch]'))
DROP VIEW [dbo].[vw_batch_NextAvailableBatch]
GO

/****** Object:  View [dbo].[vw_batch_ReleasePolicies]    Script Date: 10/14/2013 10:20:55 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vw_batch_ReleasePolicies]'))
DROP VIEW [dbo].[vw_batch_ReleasePolicies]
GO

/****** Object:  View [dbo].[vw_batch_ReleasePolicies]    Script Date: 10/14/2013 10:20:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =================================================================================================
-- Author:      François Chabot
-- Create date: 10/14/2013
-- Description: List of all batches (i.e. EnvelopeSpecName and Partition) with their release policy
--              definitions individually applied, that is one of the release policy definitions that
--              specifically applies to a particular batch (i.e. that match a specific
--              EnvelopeSpecName and Partition couple).
-- =================================================================================================
CREATE VIEW [dbo].[vw_batch_ReleasePolicies]
AS
   WITH [Batches] AS (
      SELECT DISTINCT P.EnvelopeId, P.[Partition]
      -- important to skip past the locked rows instead of blocking current transaction until locks are released;
      -- i.e. ignore parts that are either being added or deleted, the only operations that a part should undergo
      FROM batch_Parts P WITH (READPAST)
   ),
   Rules AS (
      SELECT B.EnvelopeId,
         B.[Partition],
         ISNULL(RPD.[Partition], '0') AS PolicyPartition
      FROM [Batches] B
         LEFT OUTER JOIN batch_ReleasePolicyDefinitions RPD ON B.EnvelopeId = RPD.EnvelopeId AND B.[Partition] = RPD.[Partition]
   )
   SELECT R.EnvelopeId,
      R.[Partition],
      RPD.[Enabled],
      RPD.ReleaseOnElapsedTimeOut,
      RPD.ReleaseOnIdleTimeOut,
      RPD.ReleaseOnItemCount,
      CASE RPD.EnforceItemCountLimitOnRelease WHEN 1 THEN RPD.ReleaseOnItemCount ELSE 0 END AS ItemCountLimit
   FROM Rules R
      INNER JOIN batch_ReleasePolicyDefinitions RPD ON R.EnvelopeId = RPD.EnvelopeId AND R.PolicyPartition = RPD.[Partition]
GO
/****** Object:  View [dbo].[vw_batch_AvailableBatches]    Script Date: 10/14/2013 10:20:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =================================================================================================
-- Author:      François Chabot
-- Create date: 10/11/2013
-- Description: List of all batches that are releasable either by policy or by control message.
--              This view should be used for strict monitoring purposes only, and no other views or
--              procedures should ever depend on it.
-- =================================================================================================
CREATE VIEW [dbo].[vw_batch_AvailableBatches]
AS
   WITH [Batches] AS (
      SELECT EnvelopeId,
         [Partition],
         DATEDIFF(MINUTE, MIN([Timestamp]), SYSUTCDATETIME()) AS ElapsedTime,
         DATEDIFF(MINUTE, MAX([Timestamp]), SYSUTCDATETIME()) AS IdleTime,
         COUNT(1) AS ItemCount
      FROM batch_Parts WITH (READPAST)
      GROUP BY EnvelopeId, [Partition]
   )
   SELECT B.EnvelopeId,
      E.EnvelopeSpecName,
      B.[Partition],
      B.ElapsedTime,
      B.IdleTime,
      B.ItemCount,
      RP.ItemCountLimit,
      QCR.ProcessActivityId,
      QCR.[Timestamp]
   FROM [Batches] B WITH (READPAST)
      INNER JOIN batch_Envelopes E WITH (READPAST) ON B.EnvelopeId = E.Id
      INNER JOIN vw_batch_ReleasePolicies RP WITH (READPAST) ON B.EnvelopeId = RP.EnvelopeId AND B.[Partition] = RP.[Partition]
      LEFT OUTER JOIN batch_QueuedControlledReleases QCR WITH (READPAST) ON B.EnvelopeId = QCR.EnvelopeId AND B.[Partition] = QCR.[Partition]
   WHERE RP.[Enabled] = 1 
      AND ((RP.ReleaseOnElapsedTimeOut IS NOT NULL AND B.ElapsedTime >= RP.ReleaseOnElapsedTimeOut)
         OR (RP.ReleaseOnIdleTimeOut IS NOT NULL AND B.IdleTime >= RP.ReleaseOnIdleTimeOut)
         OR (RP.ReleaseOnItemCount IS NOT NULL AND B.ItemCount >= RP.ReleaseOnItemCount)
         OR (QCR.EnvelopeId IS NOT NULL))
GO
/****** Object:  View [dbo].[vw_batch_NextAvailableBatch]    Script Date: 10/15/2013 20:44:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =================================================================================================
-- Author:      François Chabot
-- Create date: 10/11/2013
-- Description: Somehow peek or pop the top of the queue of batches available to release either by
--              policy or by control message.
-- =================================================================================================
CREATE VIEW [dbo].[vw_batch_NextAvailableBatch]
AS
   WITH [Batches] AS (
      SELECT EnvelopeId,
         [Partition],
         DATEDIFF(MINUTE, MIN([Timestamp]), SYSUTCDATETIME()) AS ElapsedTime,
         DATEDIFF(MINUTE, MAX([Timestamp]), SYSUTCDATETIME()) AS IdleTime,
         COUNT(1) AS ItemCount
      -- important to skip past the locked rows instead of blocking current transaction until locks are released;
      -- i.e. ignore parts that are either being added or deleted, the only operations that a part should undergo
      FROM batch_Parts WITH (READPAST)
      GROUP BY EnvelopeId, [Partition]
   )
   SELECT TOP 1
      B.EnvelopeId,
      E.EnvelopeSpecName,
      B.[Partition],
      B.ElapsedTime,
      B.IdleTime,
      B.ItemCount,
      RP.ItemCountLimit,
      QCR.ProcessActivityId,
      QCR.[Timestamp]
   FROM [Batches] B
      -- set XLOCK ROWLOCK on batch_Envelopes row at polling time to prevent any other concurrent polling receive
      -- location from releasing the same batch; technically the envelope is too granular a lock, but there is no
      -- easy way to set one on an EnvelopeSpecName/Partition couple as the list of partitions may vary with time
      -- and not all of them have to be registered
      INNER JOIN batch_Envelopes E WITH (READPAST, ROWLOCK, XLOCK) ON B.EnvelopeId = E.Id
      INNER JOIN vw_batch_ReleasePolicies RP ON B.EnvelopeId = RP.EnvelopeId AND B.[Partition] = RP.[Partition]
      LEFT OUTER JOIN batch_QueuedControlledReleases QCR ON B.EnvelopeId = QCR.EnvelopeId AND B.[Partition] = QCR.[Partition]
   WHERE RP.[Enabled] = 1 
      AND ((RP.ReleaseOnElapsedTimeOut IS NOT NULL AND B.ElapsedTime >= RP.ReleaseOnElapsedTimeOut)
         OR (RP.ReleaseOnIdleTimeOut IS NOT NULL AND B.IdleTime >= RP.ReleaseOnIdleTimeOut)
         OR (RP.ReleaseOnItemCount IS NOT NULL AND B.ItemCount >= RP.ReleaseOnItemCount)
         OR (QCR.EnvelopeId IS NOT NULL))
GO

/****** Object:  StoredProcedure [dbo].[usp_batch_Register]    Script Date: 10/11/2013 12:46:06 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_batch_Register]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_batch_Register]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =================================================================================================
-- Author:      François Chabot
-- Create date: 10/11/2013
-- Description: Register a batch and configure its release policy.
-- =================================================================================================
CREATE PROCEDURE [dbo].[usp_batch_Register] 
   @envelopeSpecName nvarchar(256),
   @partition nvarchar(128) = '0',
   @enabled bit,
   @releaseOnElapsedTimeOut int = null,
   @releaseOnIdleTimeOut int = null,
   @releaseOnItemCount int = null,
   @enforceItemCountLimitOnRelease bit = 0
AS
BEGIN
   -- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
   SET NOCOUNT ON;

   -- register batch's EnvelopeSpecName
   WITH [Envelopes] AS (
      SELECT @envelopeSpecName AS [EnvelopeSpecName]
   )
   MERGE INTO [batch_Envelopes] WITH (HOLDLOCK) AS [TARGET]
      USING [Envelopes] AS [SOURCE]
      ON [TARGET].[EnvelopeSpecName] = [SOURCE].[EnvelopeSpecName]
   WHEN NOT MATCHED THEN
      INSERT ([EnvelopeSpecName])
      VALUES ([SOURCE].[EnvelopeSpecName]);

   -- configure batch's release policy
   WITH [ReleasePolicyDefinitions] AS (
      SELECT [Id] AS [EnvelopeId],
         @partition AS [Partition],
         @enabled AS [Enabled],
         @releaseOnElapsedTimeOut AS [ReleaseOnElapsedTimeOut],
         @releaseOnIdleTimeOut AS [ReleaseOnIdleTimeOut],
         @releaseOnItemCount AS [ReleaseOnItemCount],
         @enforceItemCountLimitOnRelease AS [EnforceItemCountLimitOnRelease]
      FROM [batch_Envelopes]
      WHERE EnvelopeSpecName = @envelopeSpecName
   )
   MERGE INTO [batch_ReleasePolicyDefinitions] WITH (HOLDLOCK) AS [TARGET]
      USING [ReleasePolicyDefinitions] AS [SOURCE]
      ON [TARGET].[EnvelopeId] = [SOURCE].[EnvelopeId] AND [TARGET].[Partition] = [SOURCE].[Partition]
   WHEN MATCHED THEN
      UPDATE SET [Enabled] = [SOURCE].[Enabled],
         [ReleaseOnElapsedTimeOut] = [SOURCE].[ReleaseOnElapsedTimeOut],
         [ReleaseOnIdleTimeOut] = [SOURCE].[ReleaseOnIdleTimeOut],
         [ReleaseOnItemCount] = [SOURCE].[ReleaseOnItemCount],
         [EnforceItemCountLimitOnRelease] = [SOURCE].[EnforceItemCountLimitOnRelease]
   WHEN NOT MATCHED THEN
      INSERT ([EnvelopeId],
         [Partition],
         [Enabled],
         [ReleaseOnElapsedTimeOut],
         [ReleaseOnIdleTimeOut],
         [ReleaseOnItemCount],
         [EnforceItemCountLimitOnRelease])
      VALUES ([SOURCE].[EnvelopeId],
         [SOURCE].[Partition],
         [SOURCE].[Enabled],
         [SOURCE].[ReleaseOnElapsedTimeOut],
         [SOURCE].[ReleaseOnIdleTimeOut],
         [SOURCE].[ReleaseOnItemCount],
         [SOURCE].[EnforceItemCountLimitOnRelease]);
END
GO

/****** Object:  StoredProcedure [dbo].[usp_batch_Unregister]    Script Date: 10/11/2013 12:46:06 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_batch_Unregister]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_batch_Unregister]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =================================================================================================
-- Author:      François Chabot
-- Create date: 10/11/2013
-- Description: Unregister a batch and cleanup its release policy.
-- =================================================================================================
CREATE PROCEDURE [dbo].[usp_batch_Unregister] 
   @envelopeSpecName nvarchar(256),
   @partition nvarchar(128) = '0'
AS
BEGIN
   -- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
   SET NOCOUNT ON;

   -- clean batch's release policy
   DELETE [batch_ReleasePolicyDefinitions]
      FROM [batch_ReleasePolicyDefinitions] RPD INNER JOIN [batch_Envelopes] E ON RPD.EnvelopeId = E.Id
      WHERE E.EnvelopeSpecName = @envelopeSpecName
         AND RPD.[Partition] = @partition;

   -- unregister batch's EnvelopeSpecName
   DELETE [batch_Envelopes]
      FROM [batch_Envelopes] E LEFT OUTER JOIN [batch_ReleasePolicyDefinitions] RPD ON E.Id = RPD.EnvelopeId
      WHERE E.EnvelopeSpecName = @envelopeSpecName
         AND RPD.EnvelopeId IS NULL;
END
GO

/****** Object:  StoredProcedure [dbo].[usp_batch_AddPart]    Script Date: 10/11/2013 12:46:06 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_batch_AddPart]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_batch_AddPart]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =================================================================================================
-- Author:      François Chabot
-- Create date: 10/11/2013
-- Description: Add a part to a batch.
-- =================================================================================================
CREATE PROCEDURE [dbo].[usp_batch_AddPart] 
   @envelopeSpecName nvarchar(256), 
   @partition nvarchar(128) = '0',
   @messagingStepActivityId nvarchar(32) = null,
   @data nvarchar(max)
AS
BEGIN
   -- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
   SET NOCOUNT ON;

   DECLARE @envelopeId int = NULL;
   SELECT @envelopeId = id
   -- disregarding the usp_batch_Register and usp_batch_Unregister procedures, lock on batch_Envelopes should
   -- never be taken except when releasing a batch in order to prevent concurrent release attempts, see the
   -- vw_batch_NextAvailableBatch view. Releasing a batch should however not block, or delay, new parts that
   -- are being added. To put it differently, once registered, an envelope will only ever be read and read
   -- access should never be blocked or delayed by a lock.
   FROM batch_Envelopes WITH (READUNCOMMITTED)
   WHERE EnvelopeSpecName = @envelopeSpecName;

   IF @envelopeId IS NULL
   BEGIN
      RAISERROR ('Cannot find a batch with an EnvelopeSpecName of ''%s'' to which to add the part.', 16, 1, @envelopeSpecName) WITH NOWAIT
      RETURN
   END;

   INSERT INTO batch_Parts (EnvelopeId, MessagingStepActivityId, [Partition], Data)
      VALUES (@envelopeId, @messagingStepActivityId, @partition, @data);
END
GO
GRANT EXECUTE ON [dbo].[usp_batch_AddPart] TO [BTS_USERS] AS [dbo]
GO

/****** Object:  StoredProcedure [dbo].[usp_batch_QueueControlledRelease]    Script Date: 10/11/2013 12:46:06 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_batch_QueueControlledRelease]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_batch_QueueControlledRelease]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =================================================================================================
-- Author:      François Chabot
-- Create date: 10/13/2013
-- Description: Upon reception of a control message, queue a batch to be released at next polling
--              provided that it is enabled by policy and has some parts. If @envelopeSpecName
--              and/or @partition are '*' then all envelopes and/or partitions are queued to be
--              released.
-- =================================================================================================
CREATE PROCEDURE [dbo].[usp_batch_QueueControlledRelease] 
   @envelopeSpecName nvarchar(256),
   @partition nvarchar(128) = '0',
   @processActivityId nvarchar(32) = null
AS
BEGIN
   -- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
   SET NOCOUNT ON;

   WITH Envelopes AS (
      SELECT Id
      -- disregarding the usp_batch_Register and usp_batch_Unregister procedures, lock on batch_Envelopes should
      -- never be taken except when releasing a batch in order to prevent concurrent release attempts, see the
      -- vw_batch_NextAvailableBatch view. Releasing a batch should however not block, or delay, new parts that
      -- are being added. To put it differently, once registered, an envelope will only ever be read and read
      -- access should never be blocked or delayed by a lock.
      FROM batch_Envelopes WITH (READUNCOMMITTED)
      WHERE EnvelopeSpecName = @envelopeSpecName OR @envelopeSpecName = '*'
   ),
   [Batches] AS (
      SELECT RP.EnvelopeId, RP.[Partition]
      -- the vw_batch_ReleasePolicies view ensures that only batches with parts are taken into account
      FROM vw_batch_ReleasePolicies RP
         INNER JOIN Envelopes E ON RP.EnvelopeId = E.Id
      WHERE RP.[Enabled] = 1 AND (RP.[Partition] = @partition OR @partition = '*')
   )
   MERGE INTO batch_QueuedControlledReleases WITH (HOLDLOCK) AS [TARGET]
      USING [Batches] AS [SOURCE]
      ON [TARGET].EnvelopeId = [SOURCE].EnvelopeId AND [TARGET].[Partition] = [SOURCE].[Partition]
   WHEN NOT MATCHED THEN
      INSERT (EnvelopeId, [Partition], ProcessActivityId)
      VALUES ([SOURCE].EnvelopeId, [SOURCE].[Partition], @processActivityId);
END
GO
GRANT EXECUTE ON [dbo].[usp_batch_QueueControlledRelease] TO [BTS_USERS] AS [dbo]
GO

/****** Object:  StoredProcedure [dbo].[usp_batch_ReleaseNextBatch]    Script Date: 10/11/2013 12:46:06 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_batch_ReleaseNextBatch]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_batch_ReleaseNextBatch]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =================================================================================================
-- Author:      François Chabot
-- Create date: 10/11/2013
-- Description: Release the content of the next available batch, see the vw_batch_NextAvailableBatch
--              view. The xml output will be structured as follows, which is compliant to the
--              Be.Stateless.BizTalk.Schemas.Xml.Batch.Content schema:
--              <bat:BatchContent xmlns:bat="urn:schemas.stateless.be:biztalk:batch:2012:12">
--                <bat:EnvelopeSpecName>...</bat:EnvelopeSpecName>
--                <bat:Partition>...</bat:Partition>
--                <bat:ProcessActivityId>...</bat:ProcessActivityId>
--                <bat:MessagingStepActivityIds>...,...,...</bat:MessagingStepActivityIds>
--                <bat:Parts>
--                  ...
--                  ...
--                  ...
--                </bat:Parts>
--              </bat:BatchContent>
-- =================================================================================================
CREATE PROCEDURE [dbo].[usp_batch_ReleaseNextBatch] 
AS
BEGIN
   -- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
   SET NOCOUNT ON;

   DECLARE @envelopeId int = NULL;
   DECLARE @envelopeSpecName nvarchar(256) = NULL;
   DECLARE @partition nvarchar(128) = NULL;
   DECLARE @processActivityId nvarchar(32) = NULL;
   DECLARE @rowCount int = 0;
   SELECT
      @envelopeId = EnvelopeId,
      @envelopeSpecName = EnvelopeSpecName,
      @partition = [Partition],
      @processActivityId = ProcessActivityId,
      @rowCount = ISNULL(ItemCountLimit, 0)
   -- an XLOCK ROWLOCK on batch_Envelopes row is set by the view to prevent concurrent release attempts
   FROM vw_batch_NextAvailableBatch

   -- enforce limit on item count via ROWCOUNT option, see http://www.devx.com/getHelpOn/10MinuteSolution/20564
   SET ROWCOUNT @rowCount
   DECLARE @Parts TABLE (PartId [int] NOT NULL);
   INSERT INTO @Parts (PartId)
      SELECT Id
      -- important to skip past the locked rows instead of blocking current transaction until locks are released;
      -- i.e. ignore parts that are either being added or deleted, the only operations that a part should undergo.
      -- besides, because an XLOCK ROWLOCK has been taken by vw_batch_NextAvailableBatch on the EnvelopeSpecName,
      -- only one single partition (i.e. a concrete batch) among all of the partitions associated to this envelope
      -- will be released at a time, and even more, no other concurrent attempts to release this batch will never
      -- occur. it is then safe to assume that the only parts that will be skipped past are the parts in the midst
      -- of being added.
      FROM batch_Parts WITH (READPAST)
      WHERE EnvelopeId = @envelopeId AND [Partition] = @partition;
   -- remove ROWCOUNT option
   SET ROWCOUNT 0

   -- fail-fast check that should never occur though...
   IF NOT EXISTS(SELECT 1 FROM @Parts)
   BEGIN
      SET @partition = NULLIF(@partition, '0')
      RAISERROR ('There are no parts to release for the batch (EnvelopeSpecName: ''%s'', Parition ''%s'').', 16, 3, @envelopeSpecName, @partition) WITH NOWAIT
      RETURN
   END;

   WITH XMLNAMESPACES (
      'urn:schemas.stateless.be:biztalk:batch:2012:12' AS bat
   ),
   Parts AS (
      SELECT MessagingStepActivityId, Data
      -- don't mind any lock that would exist as a snapshot temporary table has been previously taken
      FROM batch_Parts WITH (READUNCOMMITTED)
         INNER JOIN @Parts ON Id = PartId
   )
   SELECT @envelopeSpecName AS 'bat:EnvelopeSpecName',
      NULLIF(@partition, '0') AS 'bat:Partition',
      @processActivityId AS 'bat:ProcessActivityId',
      STUFF((SELECT ',' + CONVERT(nvarchar(32), MessagingStepActivityId)
         FROM Parts
         FOR XML PATH ('')), 1, 1, '') AS 'bat:MessagingStepActivityIds',
      (SELECT CONVERT(XML, Data)
         FROM Parts
         FOR XML PATH (''), TYPE) AS 'bat:Parts'
   FOR XML PATH ('bat:BatchContent');

   DELETE FROM batch_QueuedControlledReleases
      WHERE EnvelopeId = @envelopeId AND [Partition] = @partition;

   -- Concurrent usp_batch_ReleaseNextBatch executions will serialize at this point; it is important to take an exclusive
   -- table lock to prevent this procedure's concurrent executions from deadlocking each others. Without the table lock,
   -- they would take row-level locks that would eventually be promoted to page-level locks ---because of a potentially
   -- large number of parts that need to be grabbed for the release of a batch;--- these page locks ownership would be
   -- spread across all of this procedure's concurrent executions and would most probably deadlock each others.
   DELETE P FROM batch_Parts P WITH (TABLOCK, XLOCK)
      INNER JOIN @Parts ON P.Id = PartId;
END
GO
GRANT EXECUTE ON [dbo].[usp_batch_ReleaseNextBatch] TO [BTS_USERS] AS [dbo]
GO

/*
        CCCCCCCCCCCCClllllll                     iiii                                  CCCCCCCCCCCCChhhhhhh                                                     kkkkkkkk           
     CCC::::::::::::Cl:::::l                    i::::i                              CCC::::::::::::Ch:::::h                                                     k::::::k           
   CC:::::::::::::::Cl:::::l                     iiii                             CC:::::::::::::::Ch:::::h                                                     k::::::k           
  C:::::CCCCCCCC::::Cl:::::l                                                     C:::::CCCCCCCC::::Ch:::::h                                                     k::::::k           
 C:::::C       CCCCCC l::::l   aaaaaaaaaaaaa   iiiiiii    mmmmmmm    mmmmmmm    C:::::C       CCCCCC h::::h hhhhh           eeeeeeeeeeee        cccccccccccccccc k:::::k    kkkkkkk
C:::::C               l::::l   a::::::::::::a  i:::::i  mm:::::::m  m:::::::mm C:::::C               h::::hh:::::hhh      ee::::::::::::ee    cc:::::::::::::::c k:::::k   k:::::k 
C:::::C               l::::l   aaaaaaaaa:::::a  i::::i m::::::::::mm::::::::::mC:::::C               h::::::::::::::hh   e::::::eeeee:::::ee c:::::::::::::::::c k:::::k  k:::::k  
C:::::C               l::::l            a::::a  i::::i m::::::::::::::::::::::mC:::::C               h:::::::hhh::::::h e::::::e     e:::::ec:::::::cccccc:::::c k:::::k k:::::k   
C:::::C               l::::l     aaaaaaa:::::a  i::::i m:::::mmm::::::mmm:::::mC:::::C               h::::::h   h::::::he:::::::eeeee::::::ec::::::c     ccccccc k::::::k:::::k    
C:::::C               l::::l   aa::::::::::::a  i::::i m::::m   m::::m   m::::mC:::::C               h:::::h     h:::::he:::::::::::::::::e c:::::c              k:::::::::::k     
C:::::C               l::::l  a::::aaaa::::::a  i::::i m::::m   m::::m   m::::mC:::::C               h:::::h     h:::::he::::::eeeeeeeeeee  c:::::c              k:::::::::::k     
 C:::::C       CCCCCC l::::l a::::a    a:::::a  i::::i m::::m   m::::m   m::::m C:::::C       CCCCCC h:::::h     h:::::he:::::::e           c::::::c     ccccccc k::::::k:::::k    
  C:::::CCCCCCCC::::Cl::::::la::::a    a:::::a i::::::im::::m   m::::m   m::::m  C:::::CCCCCCCC::::C h:::::h     h:::::he::::::::e          c:::::::cccccc:::::ck::::::k k:::::k   
   CC:::::::::::::::Cl::::::la:::::aaaa::::::a i::::::im::::m   m::::m   m::::m   CC:::::::::::::::C h:::::h     h:::::h e::::::::eeeeeeee   c:::::::::::::::::ck::::::k  k:::::k  
     CCC::::::::::::Cl::::::l a::::::::::aa:::ai::::::im::::m   m::::m   m::::m     CCC::::::::::::C h:::::h     h:::::h  ee:::::::::::::e    cc:::::::::::::::ck::::::k   k:::::k 
        CCCCCCCCCCCCCllllllll  aaaaaaaaaa  aaaaiiiiiiiimmmmmm   mmmmmm   mmmmmm        CCCCCCCCCCCCC hhhhhhh     hhhhhhh    eeeeeeeeeeeeee      cccccccccccccccckkkkkkkk    kkkkkkk

http://patorjk.com/software/taag/#p=display&f=Doh&t=ClaimCheck
*/

IF  EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_claim_Tokens_Available]') AND parent_object_id = OBJECT_ID(N'[dbo].[claim_Tokens]'))
Begin
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_claim_Tokens_Available]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[claim_Tokens] DROP CONSTRAINT [DF_claim_Tokens_Available]
END


End
GO
IF  EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_claim_Tokens_Timestamp]') AND parent_object_id = OBJECT_ID(N'[dbo].[claim_Tokens]'))
Begin
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_claim_Tokens_Timestamp]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[claim_Tokens] DROP CONSTRAINT [DF_claim_Tokens_Timestamp]
END


End
GO
/****** Object:  Table [dbo].[claim_Tokens]    Script Date: 10/11/2013 15:39:40 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[claim_Tokens]') AND type in (N'U'))
DROP TABLE [dbo].[claim_Tokens]
GO
/****** Object:  Table [dbo].[claim_Tokens]    Script Date: 12/04/2017 09:44:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[claim_Tokens]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[claim_Tokens](
   [Url] [nvarchar](50) NOT NULL,
   [Available] [bit] NOT NULL,
   [CorrelationToken] [nvarchar](256) NULL,
   [EnvironmentTag] [nvarchar](256) NULL,
   [MessageType] [nvarchar](256) NULL,
   [OutboundTransportLocation] [nvarchar](256) NULL,
   [ProcessActivityId] [nvarchar](32) NULL,
   [ReceiverName] [nvarchar](256) NULL,
   [SenderName] [nvarchar](256) NULL,
   [Any] [xml] NULL,
   [Timestamp] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_claim_Tokens] PRIMARY KEY CLUSTERED 
(
   [Url] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Default [DF_claim_Tokens_Available]    Script Date: 10/11/2013 15:39:40 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_claim_Tokens_Available]') AND parent_object_id = OBJECT_ID(N'[dbo].[claim_Tokens]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_claim_Tokens_Available]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[claim_Tokens] ADD  CONSTRAINT [DF_claim_Tokens_Available]  DEFAULT ((0)) FOR [Available]
END


End
GO
/****** Object:  Default [DF_claim_Tokens_Timestamp]    Script Date: 10/11/2013 15:39:40 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_claim_Tokens_Timestamp]') AND parent_object_id = OBJECT_ID(N'[dbo].[claim_Tokens]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_claim_Tokens_Timestamp]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[claim_Tokens] ADD  CONSTRAINT [DF_claim_Tokens_Timestamp]  DEFAULT (sysutcdatetime()) FOR [Timestamp]
END


End
GO

/****** Object:  View [dbo].[vw_claim_AvailableTokens]    Script Date: 04/22/2013 13:17:30 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[vw_claim_AvailableTokens]'))
DROP VIEW [dbo].[vw_claim_AvailableTokens]
GO

/****** Object:  View [dbo].[vw_claim_AvailableTokens]    Script Date: 04/22/2013 13:17:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vw_claim_AvailableTokens]
AS
   SELECT Url
   -- important to skip past the locked rows instead of blocking current transaction until locks are released
   FROM claim_Tokens WITH (READPAST)
   WHERE Available = 1
GO

/****** Object:  StoredProcedure [dbo].[usp_claim_CheckIn]    Script Date: 04/12/2017 09:44:42 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_claim_CheckIn]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_claim_CheckIn]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =================================================================================================
-- Author:      François Chabot
-- Create date: 12/04/2017
-- Description: Check in a new claim token.
-- =================================================================================================
CREATE PROCEDURE [dbo].[usp_claim_CheckIn] 
   @correlationToken nvarchar(256) = NULL,
   @environmentTag nvarchar(256) = NULL,
   @messageType nvarchar(256) = NULL,
   @outboundTransportLocation nvarchar(256) = NULL,
   @processActivityId nvarchar(32) = NULL,
   @receiverName nvarchar(256) = NULL,
   @senderName nvarchar(256) = NULL,
   @url nvarchar(50),
   @any [xml] = NULL
AS
BEGIN
   -- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
   SET NOCOUNT ON;

   INSERT INTO claim_Tokens (Url, CorrelationToken, EnvironmentTag, MessageType, OutboundTransportLocation, ProcessActivityId, ReceiverName, SenderName, [Any])
      VALUES (@url, @correlationToken, @environmentTag, @messageType, @outboundTransportLocation, @processActivityId, @receiverName, @senderName, @any);
END
GO
GRANT EXECUTE ON [dbo].[usp_claim_CheckIn] TO [BTS_USERS] AS [dbo]
GO

/****** Object:  StoredProcedure [dbo].[usp_claim_Release]    Script Date: 04/12/2013 13:20:42 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_claim_Release]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_claim_Release]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =================================================================================================
-- Author:      François Chabot
-- Create date: 22/04/2013
-- Description: Makes a claim token available for check out.
-- =================================================================================================
CREATE PROCEDURE [dbo].[usp_claim_Release] 
   @url nvarchar(50)
AS
BEGIN
   -- SET NOCOUNT OFF so that SQL Server returns the number of affected rows to .NET
   SET NOCOUNT OFF;

   -- set XLOCK on row to prevent concurrent execution
   UPDATE claim_Tokens WITH (ROWLOCK, XLOCK)
      SET Available = 1
      WHERE Url = @url;
END
GO
GRANT EXECUTE ON [dbo].[usp_claim_Release] TO [BTS_USERS] AS [dbo]
GO

/****** Object:  StoredProcedure [dbo].[usp_claim_CheckOut]    Script Date: 04/12/2017 09:44:42 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_claim_CheckOut]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_claim_CheckOut]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =================================================================================================
-- Author:      François Chabot
-- Create date: 12/04/2017
-- Description: Check out available claim tokens.
--              The output, which has to match the Be.Stateless.BizTalk.Schemas.Xml.Claim.Tokens
--              schema, will be as follows:
--              <env:Envelope xmlns:env="urn:schemas.stateless.be:biztalk:envelope:2013:07" xmlns:clm="urn:schemas.stateless.be:biztalk:claim:2017:04">
--                <clm:CheckOut>
--                  <clm:CorrelationToken>...</clm:CorrelationToken>
--                  <clm:EnvironmentTag>...</clm:EnvironmentTag>
--                  <clm:MessageType>...</clm:MessageType>
--                  <clm:OutboundTransportLocation>...</clm:OutboundTransportLocation>
--                  <clm:ProcessActivityId>...</clm:ProcessActivityId>
--                  <clm:ReceiverName>...</clm:ReceiverName>
--                  <clm:SenderName>...</clm:SenderName>
--                  <clm:Url>...</clm:Url>
--                  <Any>...</Any>
--                </clm:CheckOut>
--                <clm:CheckOut>
--                  <clm:CorrelationToken>...</clm:CorrelationToken>
--                  <clm:ReceiverName>...</clm:ReceiverName>
--                  <clm:Url>...</clm:Url>
--                  <Any>...</Any>
--                </clm:CheckOut>
--                <clm:CheckOut>
--                  <clm:Url>...</clm:Url>
--                </clm:CheckOut>
--              </env:Envelope>
-- =================================================================================================
CREATE PROCEDURE [dbo].[usp_claim_CheckOut] 
AS
BEGIN
   -- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
   SET NOCOUNT ON;

   DECLARE @AvailableTokens TABLE (Url nvarchar(50) NOT NULL);
   INSERT INTO @AvailableTokens (Url)
      SELECT CT.Url
      -- set XLOCK on row to prevent concurrent execution
      FROM claim_Tokens CT WITH (ROWLOCK, XLOCK)
         INNER JOIN vw_claim_AvailableTokens AT ON CT.Url = AT.Url;

   WITH XMLNAMESPACES (
      'urn:schemas.stateless.be:biztalk:envelope:2013:07' AS env,
      'urn:schemas.stateless.be:biztalk:claim:2017:04' AS clm
   )
   SELECT CT.[CorrelationToken] AS 'clm:CorrelationToken',
      CT.[EnvironmentTag] AS 'clm:EnvironmentTag',
      CT.[MessageType] AS 'clm:MessageType',
      CT.[OutboundTransportLocation] AS 'clm:OutboundTransportLocation',
      CT.[ProcessActivityId] AS 'clm:ProcessActivityId',
      CT.[ReceiverName] AS 'clm:ReceiverName',
      CT.[SenderName] AS 'clm:SenderName',
      CT.[Url] AS 'clm:Url',
      CONVERT(XML, CT.[Any])
   FROM claim_Tokens CT
      INNER JOIN @AvailableTokens AT ON CT.Url = AT.Url
   FOR XML PATH ('clm:CheckOut'), ROOT('env:Envelope');

   DELETE CT FROM claim_Tokens CT WITH (TABLOCK, XLOCK)
      INNER JOIN @AvailableTokens AT ON CT.Url = AT.Url;
END
GO
GRANT EXECUTE ON [dbo].[usp_claim_CheckOut] TO [BTS_USERS] AS [dbo]
GO


/*
     QQQQQQQQQ                                                                     tttt                           
   QQ:::::::::QQ                                                                ttt:::t                           
 QQ:::::::::::::QQ                                                              t:::::t                           
Q:::::::QQQ:::::::Q                                                             t:::::t                           
Q::::::O   Q::::::Quuuuuu    uuuuuu    aaaaaaaaaaaaa  rrrrr   rrrrrrrrr   ttttttt:::::ttttttt    zzzzzzzzzzzzzzzzz
Q:::::O     Q:::::Qu::::u    u::::u    a::::::::::::a r::::rrr:::::::::r  t:::::::::::::::::t    z:::::::::::::::z
Q:::::O     Q:::::Qu::::u    u::::u    aaaaaaaaa:::::ar:::::::::::::::::r t:::::::::::::::::t    z::::::::::::::z 
Q:::::O     Q:::::Qu::::u    u::::u             a::::arr::::::rrrrr::::::rtttttt:::::::tttttt    zzzzzzzz::::::z  
Q:::::O     Q:::::Qu::::u    u::::u      aaaaaaa:::::a r:::::r     r:::::r      t:::::t                z::::::z   
Q:::::O     Q:::::Qu::::u    u::::u    aa::::::::::::a r:::::r     rrrrrrr      t:::::t               z::::::z    
Q:::::O  QQQQ:::::Qu::::u    u::::u   a::::aaaa::::::a r:::::r                  t:::::t              z::::::z     
Q::::::O Q::::::::Qu:::::uuuu:::::u  a::::a    a:::::a r:::::r                  t:::::t    tttttt   z::::::z      
Q:::::::QQ::::::::Qu:::::::::::::::uua::::a    a:::::a r:::::r                  t::::::tttt:::::t  z::::::zzzzzzzz
 QQ::::::::::::::Q  u:::::::::::::::ua:::::aaaa::::::a r:::::r                  tt::::::::::::::t z::::::::::::::z
   QQ:::::::::::Q    uu::::::::uu:::u a::::::::::aa:::ar:::::r                    tt:::::::::::ttz:::::::::::::::z
     QQQQQQQQ::::QQ    uuuuuuuu  uuuu  aaaaaaaaaa  aaaarrrrrrr                      ttttttttttt  zzzzzzzzzzzzzzzzz
             Q:::::Q                                                                                              
              QQQQQQ                                                                                              
*/

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[FK_QRTZ_TRIGGERS_QRTZ_JOB_DETAILS]') AND OBJECTPROPERTY(id, N'ISFOREIGNKEY') = 1)
ALTER TABLE [dbo].[QRTZ_TRIGGERS] DROP CONSTRAINT FK_QRTZ_TRIGGERS_QRTZ_JOB_DETAILS
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[FK_QRTZ_CRON_TRIGGERS_QRTZ_TRIGGERS]') AND OBJECTPROPERTY(id, N'ISFOREIGNKEY') = 1)
ALTER TABLE [dbo].[QRTZ_CRON_TRIGGERS] DROP CONSTRAINT FK_QRTZ_CRON_TRIGGERS_QRTZ_TRIGGERS
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[FK_QRTZ_SIMPLE_TRIGGERS_QRTZ_TRIGGERS]') AND OBJECTPROPERTY(id, N'ISFOREIGNKEY') = 1)
ALTER TABLE [dbo].[QRTZ_SIMPLE_TRIGGERS] DROP CONSTRAINT FK_QRTZ_SIMPLE_TRIGGERS_QRTZ_TRIGGERS
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[FK_QRTZ_SIMPROP_TRIGGERS_QRTZ_TRIGGERS]') AND OBJECTPROPERTY(id, N'ISFOREIGNKEY') = 1)
ALTER TABLE [dbo].[QRTZ_SIMPROP_TRIGGERS] DROP CONSTRAINT FK_QRTZ_SIMPROP_TRIGGERS_QRTZ_TRIGGERS
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_QRTZ_JOB_LISTENERS_QRTZ_JOB_DETAILS]') AND parent_object_id = OBJECT_ID(N'[dbo].[QRTZ_JOB_LISTENERS]'))
ALTER TABLE [dbo].[QRTZ_JOB_LISTENERS] DROP CONSTRAINT [FK_QRTZ_JOB_LISTENERS_QRTZ_JOB_DETAILS]

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_QRTZ_TRIGGER_LISTENERS_QRTZ_TRIGGERS]') AND parent_object_id = OBJECT_ID(N'[dbo].[QRTZ_TRIGGER_LISTENERS]'))
ALTER TABLE [dbo].[QRTZ_TRIGGER_LISTENERS] DROP CONSTRAINT [FK_QRTZ_TRIGGER_LISTENERS_QRTZ_TRIGGERS]


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[QRTZ_CALENDARS]') AND OBJECTPROPERTY(id, N'ISUSERTABLE') = 1)
DROP TABLE [dbo].[QRTZ_CALENDARS]
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[QRTZ_CRON_TRIGGERS]') AND OBJECTPROPERTY(id, N'ISUSERTABLE') = 1)
DROP TABLE [dbo].[QRTZ_CRON_TRIGGERS]
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[QRTZ_BLOB_TRIGGERS]') AND OBJECTPROPERTY(id, N'ISUSERTABLE') = 1)
DROP TABLE [dbo].[QRTZ_BLOB_TRIGGERS]
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[QRTZ_FIRED_TRIGGERS]') AND OBJECTPROPERTY(id, N'ISUSERTABLE') = 1)
DROP TABLE [dbo].[QRTZ_FIRED_TRIGGERS]
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[QRTZ_PAUSED_TRIGGER_GRPS]') AND OBJECTPROPERTY(id, N'ISUSERTABLE') = 1)
DROP TABLE [dbo].[QRTZ_PAUSED_TRIGGER_GRPS]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QRTZ_JOB_LISTENERS]') AND type in (N'U'))
DROP TABLE [dbo].[QRTZ_JOB_LISTENERS]

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[QRTZ_SCHEDULER_STATE]') AND OBJECTPROPERTY(id, N'ISUSERTABLE') = 1)
DROP TABLE [dbo].[QRTZ_SCHEDULER_STATE]
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[QRTZ_LOCKS]') AND OBJECTPROPERTY(id, N'ISUSERTABLE') = 1)
DROP TABLE [dbo].[QRTZ_LOCKS]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QRTZ_TRIGGER_LISTENERS]') AND type in (N'U'))
DROP TABLE [dbo].[QRTZ_TRIGGER_LISTENERS]


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[QRTZ_JOB_DETAILS]') AND OBJECTPROPERTY(id, N'ISUSERTABLE') = 1)
DROP TABLE [dbo].[QRTZ_JOB_DETAILS]
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[QRTZ_SIMPLE_TRIGGERS]') AND OBJECTPROPERTY(id, N'ISUSERTABLE') = 1)
DROP TABLE [dbo].[QRTZ_SIMPLE_TRIGGERS]
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[QRTZ_SIMPROP_TRIGGERS]') AND OBJECTPROPERTY(id, N'ISUSERTABLE') = 1)
DROP TABLE [dbo].QRTZ_SIMPROP_TRIGGERS
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[QRTZ_TRIGGERS]') AND OBJECTPROPERTY(id, N'ISUSERTABLE') = 1)
DROP TABLE [dbo].[QRTZ_TRIGGERS]
GO

CREATE TABLE [dbo].[QRTZ_CALENDARS] (
  [SCHED_NAME] [NVARCHAR] (100)  NOT NULL ,
  [CALENDAR_NAME] [NVARCHAR] (200)  NOT NULL ,
  [CALENDAR] [IMAGE] NOT NULL
)
GO

CREATE TABLE [dbo].[QRTZ_CRON_TRIGGERS] (
  [SCHED_NAME] [NVARCHAR] (100)  NOT NULL ,
  [TRIGGER_NAME] [NVARCHAR] (150)  NOT NULL ,
  [TRIGGER_GROUP] [NVARCHAR] (150)  NOT NULL ,
  [CRON_EXPRESSION] [NVARCHAR] (120)  NOT NULL ,
  [TIME_ZONE_ID] [NVARCHAR] (80) 
)
GO

CREATE TABLE [dbo].[QRTZ_FIRED_TRIGGERS] (
  [SCHED_NAME] [NVARCHAR] (100)  NOT NULL ,
  [ENTRY_ID] [NVARCHAR] (95)  NOT NULL ,
  [TRIGGER_NAME] [NVARCHAR] (150)  NOT NULL ,
  [TRIGGER_GROUP] [NVARCHAR] (150)  NOT NULL ,
  [INSTANCE_NAME] [NVARCHAR] (200)  NOT NULL ,
  [FIRED_TIME] [BIGINT] NOT NULL ,
  [SCHED_TIME] [BIGINT] NOT NULL ,
  [PRIORITY] [INTEGER] NOT NULL ,
  [STATE] [NVARCHAR] (16)  NOT NULL,
  [JOB_NAME] [NVARCHAR] (150)  NULL ,
  [JOB_GROUP] [NVARCHAR] (150)  NULL ,
  [IS_NONCONCURRENT] BIT  NULL ,
  [REQUESTS_RECOVERY] BIT  NULL 
)
GO

CREATE TABLE [dbo].[QRTZ_PAUSED_TRIGGER_GRPS] (
  [SCHED_NAME] [NVARCHAR] (100)  NOT NULL ,
  [TRIGGER_GROUP] [NVARCHAR] (150)  NOT NULL 
)
GO

CREATE TABLE [dbo].[QRTZ_SCHEDULER_STATE] (
  [SCHED_NAME] [NVARCHAR] (100)  NOT NULL ,
  [INSTANCE_NAME] [NVARCHAR] (200)  NOT NULL ,
  [LAST_CHECKIN_TIME] [BIGINT] NOT NULL ,
  [CHECKIN_INTERVAL] [BIGINT] NOT NULL
)
GO

CREATE TABLE [dbo].[QRTZ_LOCKS] (
  [SCHED_NAME] [NVARCHAR] (100)  NOT NULL ,
  [LOCK_NAME] [NVARCHAR] (40)  NOT NULL 
)
GO

CREATE TABLE [dbo].[QRTZ_JOB_DETAILS] (
  [SCHED_NAME] [NVARCHAR] (100)  NOT NULL ,
  [JOB_NAME] [NVARCHAR] (150)  NOT NULL ,
  [JOB_GROUP] [NVARCHAR] (150)  NOT NULL ,
  [DESCRIPTION] [NVARCHAR] (250) NULL ,
  [JOB_CLASS_NAME] [NVARCHAR] (250)  NOT NULL ,
  [IS_DURABLE] BIT  NOT NULL ,
  [IS_NONCONCURRENT] BIT  NOT NULL ,
  [IS_UPDATE_DATA] BIT  NOT NULL ,
  [REQUESTS_RECOVERY] BIT  NOT NULL ,
  [JOB_DATA] [IMAGE] NULL
)
GO

CREATE TABLE [dbo].[QRTZ_SIMPLE_TRIGGERS] (
  [SCHED_NAME] [NVARCHAR] (100)  NOT NULL ,
  [TRIGGER_NAME] [NVARCHAR] (150)  NOT NULL ,
  [TRIGGER_GROUP] [NVARCHAR] (150)  NOT NULL ,
  [REPEAT_COUNT] [INTEGER] NOT NULL ,
  [REPEAT_INTERVAL] [BIGINT] NOT NULL ,
  [TIMES_TRIGGERED] [INTEGER] NOT NULL
)
GO

CREATE TABLE [dbo].[QRTZ_SIMPROP_TRIGGERS] (
  [SCHED_NAME] [NVARCHAR] (100)  NOT NULL ,
  [TRIGGER_NAME] [NVARCHAR] (150)  NOT NULL ,
  [TRIGGER_GROUP] [NVARCHAR] (150)  NOT NULL ,
  [STR_PROP_1] [NVARCHAR] (512) NULL,
  [STR_PROP_2] [NVARCHAR] (512) NULL,
  [STR_PROP_3] [NVARCHAR] (512) NULL,
  [INT_PROP_1] [INT] NULL,
  [INT_PROP_2] [INT] NULL,
  [LONG_PROP_1] [BIGINT] NULL,
  [LONG_PROP_2] [BIGINT] NULL,
  [DEC_PROP_1] [NUMERIC] (13,4) NULL,
  [DEC_PROP_2] [NUMERIC] (13,4) NULL,
  [BOOL_PROP_1] BIT NULL,
  [BOOL_PROP_2] BIT NULL,
)
GO

CREATE TABLE [dbo].[QRTZ_BLOB_TRIGGERS] (
  [SCHED_NAME] [NVARCHAR] (100)  NOT NULL ,
  [TRIGGER_NAME] [NVARCHAR] (150)  NOT NULL ,
  [TRIGGER_GROUP] [NVARCHAR] (150)  NOT NULL ,
  [BLOB_DATA] [IMAGE] NULL
)
GO

CREATE TABLE [dbo].[QRTZ_TRIGGERS] (
  [SCHED_NAME] [NVARCHAR] (100)  NOT NULL ,
  [TRIGGER_NAME] [NVARCHAR] (150)  NOT NULL ,
  [TRIGGER_GROUP] [NVARCHAR] (150)  NOT NULL ,
  [JOB_NAME] [NVARCHAR] (150)  NOT NULL ,
  [JOB_GROUP] [NVARCHAR] (150)  NOT NULL ,
  [DESCRIPTION] [NVARCHAR] (250) NULL ,
  [NEXT_FIRE_TIME] [BIGINT] NULL ,
  [PREV_FIRE_TIME] [BIGINT] NULL ,
  [PRIORITY] [INTEGER] NULL ,
  [TRIGGER_STATE] [NVARCHAR] (16)  NOT NULL ,
  [TRIGGER_TYPE] [NVARCHAR] (8)  NOT NULL ,
  [START_TIME] [BIGINT] NOT NULL ,
  [END_TIME] [BIGINT] NULL ,
  [CALENDAR_NAME] [NVARCHAR] (200)  NULL ,
  [MISFIRE_INSTR] [INTEGER] NULL ,
  [JOB_DATA] [IMAGE] NULL
)
GO

ALTER TABLE [dbo].[QRTZ_CALENDARS] WITH NOCHECK ADD
  CONSTRAINT [PK_QRTZ_CALENDARS] PRIMARY KEY  CLUSTERED
  (
    [SCHED_NAME],
    [CALENDAR_NAME]
  ) 
GO

ALTER TABLE [dbo].[QRTZ_CRON_TRIGGERS] WITH NOCHECK ADD
  CONSTRAINT [PK_QRTZ_CRON_TRIGGERS] PRIMARY KEY  CLUSTERED
  (
    [SCHED_NAME],
    [TRIGGER_NAME],
    [TRIGGER_GROUP]
  ) 
GO

ALTER TABLE [dbo].[QRTZ_FIRED_TRIGGERS] WITH NOCHECK ADD
  CONSTRAINT [PK_QRTZ_FIRED_TRIGGERS] PRIMARY KEY  CLUSTERED
  (
    [SCHED_NAME],
    [ENTRY_ID]
  ) 
GO

ALTER TABLE [dbo].[QRTZ_PAUSED_TRIGGER_GRPS] WITH NOCHECK ADD
  CONSTRAINT [PK_QRTZ_PAUSED_TRIGGER_GRPS] PRIMARY KEY  CLUSTERED
  (
    [SCHED_NAME],
    [TRIGGER_GROUP]
  ) 
GO

ALTER TABLE [dbo].[QRTZ_SCHEDULER_STATE] WITH NOCHECK ADD
  CONSTRAINT [PK_QRTZ_SCHEDULER_STATE] PRIMARY KEY  CLUSTERED
  (
    [SCHED_NAME],
    [INSTANCE_NAME]
  ) 
GO

ALTER TABLE [dbo].[QRTZ_LOCKS] WITH NOCHECK ADD
  CONSTRAINT [PK_QRTZ_LOCKS] PRIMARY KEY  CLUSTERED
  (
    [SCHED_NAME],
    [LOCK_NAME]
  ) 
GO

ALTER TABLE [dbo].[QRTZ_JOB_DETAILS] WITH NOCHECK ADD
  CONSTRAINT [PK_QRTZ_JOB_DETAILS] PRIMARY KEY  CLUSTERED
  (
    [SCHED_NAME],
    [JOB_NAME],
    [JOB_GROUP]
  ) 
GO

ALTER TABLE [dbo].[QRTZ_SIMPLE_TRIGGERS] WITH NOCHECK ADD
  CONSTRAINT [PK_QRTZ_SIMPLE_TRIGGERS] PRIMARY KEY  CLUSTERED
  (
    [SCHED_NAME],
    [TRIGGER_NAME],
    [TRIGGER_GROUP]
  ) 
GO

ALTER TABLE [dbo].[QRTZ_SIMPROP_TRIGGERS] WITH NOCHECK ADD
  CONSTRAINT [PK_QRTZ_SIMPROP_TRIGGERS] PRIMARY KEY  CLUSTERED
  (
    [SCHED_NAME],
    [TRIGGER_NAME],
    [TRIGGER_GROUP]
  ) 
GO

ALTER TABLE [dbo].[QRTZ_TRIGGERS] WITH NOCHECK ADD
  CONSTRAINT [PK_QRTZ_TRIGGERS] PRIMARY KEY  CLUSTERED
  (
    [SCHED_NAME],
    [TRIGGER_NAME],
    [TRIGGER_GROUP]
  ) 
GO

ALTER TABLE [dbo].QRTZ_BLOB_TRIGGERS WITH NOCHECK ADD
  CONSTRAINT [PK_QRTZ_BLOB_TRIGGERS] PRIMARY KEY  CLUSTERED
  (
    [SCHED_NAME],
    [TRIGGER_NAME],
    [TRIGGER_GROUP]
  ) 
GO

ALTER TABLE [dbo].[QRTZ_CRON_TRIGGERS] ADD
  CONSTRAINT [FK_QRTZ_CRON_TRIGGERS_QRTZ_TRIGGERS] FOREIGN KEY
  (
    [SCHED_NAME],
    [TRIGGER_NAME],
    [TRIGGER_GROUP]
  ) REFERENCES [dbo].[QRTZ_TRIGGERS] (
    [SCHED_NAME],
    [TRIGGER_NAME],
    [TRIGGER_GROUP]
  ) ON DELETE CASCADE
GO

ALTER TABLE [dbo].[QRTZ_SIMPLE_TRIGGERS] ADD
  CONSTRAINT [FK_QRTZ_SIMPLE_TRIGGERS_QRTZ_TRIGGERS] FOREIGN KEY
  (
    [SCHED_NAME],
    [TRIGGER_NAME],
    [TRIGGER_GROUP]
  ) REFERENCES [dbo].[QRTZ_TRIGGERS] (
    [SCHED_NAME],
    [TRIGGER_NAME],
    [TRIGGER_GROUP]
  ) ON DELETE CASCADE
GO

ALTER TABLE [dbo].[QRTZ_SIMPROP_TRIGGERS] ADD
  CONSTRAINT [FK_QRTZ_SIMPROP_TRIGGERS_QRTZ_TRIGGERS] FOREIGN KEY
  (
	[SCHED_NAME],
    [TRIGGER_NAME],
    [TRIGGER_GROUP]
  ) REFERENCES [dbo].[QRTZ_TRIGGERS] (
    [SCHED_NAME],
    [TRIGGER_NAME],
    [TRIGGER_GROUP]
  ) ON DELETE CASCADE
GO

ALTER TABLE [dbo].[QRTZ_TRIGGERS] ADD
  CONSTRAINT [FK_QRTZ_TRIGGERS_QRTZ_JOB_DETAILS] FOREIGN KEY
  (
    [SCHED_NAME],
    [JOB_NAME],
    [JOB_GROUP]
  ) REFERENCES [dbo].[QRTZ_JOB_DETAILS] (
    [SCHED_NAME],
    [JOB_NAME],
    [JOB_GROUP]
  )
GO

CREATE INDEX IDX_QRTZ_T_J ON QRTZ_TRIGGERS(SCHED_NAME,JOB_NAME,JOB_GROUP)
CREATE INDEX IDX_QRTZ_T_JG ON QRTZ_TRIGGERS(SCHED_NAME,JOB_GROUP)
CREATE INDEX IDX_QRTZ_T_C ON QRTZ_TRIGGERS(SCHED_NAME,CALENDAR_NAME)
CREATE INDEX IDX_QRTZ_T_G ON QRTZ_TRIGGERS(SCHED_NAME,TRIGGER_GROUP)
CREATE INDEX IDX_QRTZ_T_STATE ON QRTZ_TRIGGERS(SCHED_NAME,TRIGGER_STATE)
CREATE INDEX IDX_QRTZ_T_N_STATE ON QRTZ_TRIGGERS(SCHED_NAME,TRIGGER_NAME,TRIGGER_GROUP,TRIGGER_STATE)
CREATE INDEX IDX_QRTZ_T_N_G_STATE ON QRTZ_TRIGGERS(SCHED_NAME,TRIGGER_GROUP,TRIGGER_STATE)
CREATE INDEX IDX_QRTZ_T_NEXT_FIRE_TIME ON QRTZ_TRIGGERS(SCHED_NAME,NEXT_FIRE_TIME)
CREATE INDEX IDX_QRTZ_T_NFT_ST ON QRTZ_TRIGGERS(SCHED_NAME,TRIGGER_STATE,NEXT_FIRE_TIME)
CREATE INDEX IDX_QRTZ_T_NFT_MISFIRE ON QRTZ_TRIGGERS(SCHED_NAME,MISFIRE_INSTR,NEXT_FIRE_TIME)
CREATE INDEX IDX_QRTZ_T_NFT_ST_MISFIRE ON QRTZ_TRIGGERS(SCHED_NAME,MISFIRE_INSTR,NEXT_FIRE_TIME,TRIGGER_STATE)
CREATE INDEX IDX_QRTZ_T_NFT_ST_MISFIRE_GRP ON QRTZ_TRIGGERS(SCHED_NAME,MISFIRE_INSTR,NEXT_FIRE_TIME,TRIGGER_GROUP,TRIGGER_STATE)

CREATE INDEX IDX_QRTZ_FT_TRIG_INST_NAME ON QRTZ_FIRED_TRIGGERS(SCHED_NAME,INSTANCE_NAME)
CREATE INDEX IDX_QRTZ_FT_INST_JOB_REQ_RCVRY ON QRTZ_FIRED_TRIGGERS(SCHED_NAME,INSTANCE_NAME,REQUESTS_RECOVERY)
CREATE INDEX IDX_QRTZ_FT_J_G ON QRTZ_FIRED_TRIGGERS(SCHED_NAME,JOB_NAME,JOB_GROUP)
CREATE INDEX IDX_QRTZ_FT_JG ON QRTZ_FIRED_TRIGGERS(SCHED_NAME,JOB_GROUP)
CREATE INDEX IDX_QRTZ_FT_T_G ON QRTZ_FIRED_TRIGGERS(SCHED_NAME,TRIGGER_NAME,TRIGGER_GROUP)
CREATE INDEX IDX_QRTZ_FT_TG ON QRTZ_FIRED_TRIGGERS(SCHED_NAME,TRIGGER_GROUP)
GO

/* Table used by log4net to log job execution history */
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[QRTZ_JobHistory]') AND OBJECTPROPERTY(id, N'ISUSERTABLE') = 1)
DROP TABLE [dbo].[QRTZ_JobHistory]
GO

CREATE TABLE [DBO].[QRTZ_JOBHISTORY](
	[DATE] [DATETIME] NOT NULL,
	[LEVEL] [VARCHAR](20) NOT NULL,
	[MESSAGE] [VARCHAR](4000) NOT NULL
) ON [PRIMARY]

GO

CREATE CLUSTERED INDEX IDX_QRTZ_JOBHISTORY_DATE ON QRTZ_JOBHISTORY([DATE] DESC)
GO
