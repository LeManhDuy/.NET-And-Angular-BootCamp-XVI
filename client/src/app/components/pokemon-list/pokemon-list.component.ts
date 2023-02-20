import { SelectionModel } from '@angular/cdk/collections';
import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { Pokemon } from 'src/app/models/Pokemon';
import { PokemonsService } from 'src/app/services/pokemons.service';
import { MatPaginator } from '@angular/material/paginator';

@Component({
  selector: 'app-pokemon-list',
  templateUrl: './pokemon-list.component.html',
  styleUrls: ['./pokemon-list.component.css']
})
export class PokemonListComponent implements OnInit {
  // displayedColumns: string[] = ['Name', 'BirthDate', 'CategoryName'];
  // dataSource: any;
  // selection: any;
  pokemons: Pokemon[] = [];
  constructor(private pokemonsService: PokemonsService) { }

  ngOnInit(): void {
    this.pokemonsService.getAllPokemons().subscribe({
      next: (pokemons) => {
        this.pokemons = Object.values(pokemons)[7];
        console.log(Object.values(pokemons));
      },
      error: (response) => {
        console.log(response);
      }
    });
  }

  //@ViewChild(MatPaginator) paginator: MatPaginator | undefined;

  // ngAfterViewInit(): void {
  //   this.pokemonsService.getAllPokemons().subscribe({
  //     next: (pokemonPageData) => {

  //       const pokemons = Object.values(pokemonPageData)[7];
  //       this.dataSource = new MatTableDataSource<Pokemon>(pokemons);

  //       this.selection = new SelectionModel<Pokemon>(true, []);
  //     },
  //     error: (response) => {
  //       console.log(response);
  //     }
  //   });
  // }
}