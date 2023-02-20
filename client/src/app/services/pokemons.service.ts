import { Pokemon } from 'src/app/models/Pokemon';
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

  getAllPokemons(): Observable<Pokemon> {
    return this.http.get<Pokemon>(this.baseApiUrl + '/v1/api/pokemon')
  }
}
