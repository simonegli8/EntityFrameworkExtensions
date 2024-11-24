﻿using System;
using System.Collections.Generic;
using SolidCP.EnterpriseServer.Data.Configuration;
using SolidCP.EnterpriseServer.Data.Entities;
using System.ComponentModel.DataAnnotations.Schema;
#if NetCore
using Microsoft.EntityFrameworkCore;
#endif
#if NetFX
using System.Data.Entity;
#endif

namespace SolidCP.EnterpriseServer.Data.Configuration;

public partial class ExchangeDisclaimerConfiguration : EntityTypeConfiguration<ExchangeDisclaimer>
{
	public override void Configure()
	{
		if (IsCore && IsSqlite) Property(e => e.DisclaimerName).HasColumnType("TEXT COLLATE NOCASE");
	}
}