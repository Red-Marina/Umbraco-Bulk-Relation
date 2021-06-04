BEGIN 

    MERGE INTO UmbracoRelation AS Target 
    using #TempUmbracoRelation AS Source 
    ON 
		Target.parentId = source.parentId 
		and Target.childId = source.childId 
		and Target.relType = source.relType 
    WHEN matched THEN 
      UPDATE 
		SET Target.parentId = Source.parentId,
			Target.childId = Source.childId,
			Target.relType = Source.relType,
			Target.[datetime] = Source.[datetime],
			Target.comment = Source.comment
    WHEN NOT matched THEN 
      INSERT (parentId, 
              childId, relType, [datetime], comment) 
      VALUES (Source.parentId, 
              Source.childId, Source.relType, Source.[datetime], Source.comment); 
    DELETE FROM UmbracoRelation WHERE
        parentId = @ParentId 
        AND reltype = @RelType
        AND childId NOT IN (@childIds)
END;
