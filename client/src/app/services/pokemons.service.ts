import { PokemonPageData } from './../models/PokemonPageData';
import { environment } from './../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PokemonsService {

  baseApiUrl: string = environment.baseApiUrl

  constructor(private http: HttpClient) { }

  getAllPokemons(): Observable<PokemonPageData> {
    return this.http.get<PokemonPageData>(this.baseApiUrl + '/v1/api/pokemon')
  }
}
