<%@ Assembly Name="Solita.LanguageEditor.UI" %>
<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Solita.LanguageEditor.UI.Models.LanguageEditorViewModel>" %>

The only reason this file exists, is that without it the Razor template doesn't find the Solita.LanguageEditor assembly.
Maybe it's a bug, maybe it's a feature, but it needs to be sorted out when there's more time and documentation available.