import type { ConfigFile } from "@rtk-query/codegen-openapi";

const config: ConfigFile = {
  schemaFile: "http://localhost:4998/swagger/v1/swagger.json",
  apiFile: "./EmptyAPI.ts",
  apiImport: "emptySplitApi",
  outputFile: "./MDWatch.ts",
  exportName: "MDWatchAPI",
  hooks: true,
};

export default config;
