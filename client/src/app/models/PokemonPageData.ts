import { Type } from "@angular/compiler";

export interface PokemonPageData {
  PageNumber: Int32Array,
  PageSize: Int32Array,
  FirstPage: string,
  LastPage: string,
  TotalPages: Int32Array,
  TotalRecords: Int32Array,
  NextPage: string,
  Data: Array<Type>,
}