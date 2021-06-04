  BEGIN
    DROP TABLE IF EXISTS #TempUmbracoRelation
  END;

  BEGIN
      CREATE TABLE #TempUmbracoRelation
      ( 
        [parentId] [int] NOT NULL,
	    [childId] [int] NOT NULL,
	    [relType] [int] NOT NULL,
	    [datetime] [datetime] NOT NULL,
	    [comment] [nvarchar](1000) NOT NULL,
      ); 
  END;